$PBExportHeader$nvo_performance.sru
forward
global type nvo_performance from nonvisualobject
end type
end forward

global type nvo_performance from nonvisualobject
end type
global nvo_performance nvo_performance

type variables
private:
	string is_markers[] // markers passed to uf_start() and uf_end()
	time	 it_start_times[] // the N-th index of it_start_times corresponds to the N-th index of is_markers, so the arrays act like a Dictionary
	
	constant boolean CLEAN_UP_PREV_SESSIONS = true // false -> don't delete old sessions logs, append the new session after them
	constant int MIN_MS_TO_LOG = 100 // a minimum time (in milliseconds) considered bad performance (which we want to log)
end variables

forward prototypes
public subroutine uf_end (string as_marker)
private subroutine __read_me__ ()
public function integer uf_get_marker_index (string as_marker)
public subroutine uf_start (string as_marker)
public subroutine uf_start_new_session ()
private function string uf_get_log_file ()
public subroutine uf_log (string as_msg, writemode a_write_mode)
private subroutine uf_log (string as_msg)
end prototypes

public subroutine uf_end (string as_marker);int    i
long   ll_sec
long   ll_ms
string ls_start
string ls_end
string ls_msg
time   lt_end

if as_marker = "" or IsNull(as_marker) then
	uf_log("You cannot pass empty marker to uf_end()")
	return
end if

i = uf_get_marker_index(as_marker)
if i = 0 then
	uf_log("~nYou call:~ngnv_performance.uf_end('" + as_marker + "')~nBut you didn't call gnv_performance.uf_start() for that marker.~n")
	return
end if

lt_end = Now()

ls_start = String(it_start_times[i], "hh:mm:ss.fff")
ls_end   = String(lt_end, "hh:mm:ss.fff")

ll_sec = SecondsAfter(it_start_times[i], lt_end)
ll_ms  = Long(Right(ls_end,3)) - Long(Right(ls_start,3))

if ll_sec > 0 or ll_ms >= MIN_MS_TO_LOG then
	// Handle a borrow between seconds and milliseconds:
	// it_start_times[i] = 10:00:05.900, lt_end = 10:00:06.100; we want 200, not -800:
	if ll_ms < 0 then
	   ll_ms += 1000
	   ll_sec -= 1
	end if
	
	ls_msg = Left(as_marker + Space(100), 100) + " " + Right("00" + string(ll_sec), 2) + "." + Right("000" + string(ll_ms), 3)
	uf_log(ls_msg)
end if

// Reset the marker's time; it will be validated on the next call of uf_start() - if not null, then uf_end() was forgotten to be called for the previous marker:
SetNull(it_start_times[i])
// Don't reset is_markers[i] - the slot will be reused if this marker is passed to uf_start() again
end subroutine

private subroutine __read_me__ ();/**********************************************************************************************************************************************
This object is used to measure performance of a piece of code which could be slow, like dw.Retrieve(), embedded SQL or RPCFUNC call.
***********************************************************************************************************************************************

INITIAL SETUP:

@@@ Declare a global var:
nvo_performance gnv_performance

@@@ Add to the beginning of the Application (app_diamond) Open event:
gnv_performance = create nvo_performance
gnv_performance.uf_start_new_session()

***********************************************************************************************************************************************

HOW TO USE:

To measure performance of a piece of code, ornament it with uf_start() and uf_end().
The log file will show seconds and milliseconds between uf_star() and uf_end():

gnv_performance.uf_start('nvo_my_obj.Constructor - dw_1.Retrieve()')
dw_1.Retrieve()
gnv_performance.uf_end('nvo_my_obj.Constructor - dw_1.Retrieve()')
...
...
gnv_performance.uf_start('nvo_my_obj.Constructor - SELECT ...')
SELECT ... INTO ... FROM ...;
gnv_performance.uf_end('nvo_my_obj.Constructor - SELECT ...')

IMPORTANT! A same marker must be sent to the both! To ensure that, you can use a variable:

string ls_marker = 'nvo_my_obj.Constructor - dw_1.Retrieve()'
gnv_performance.uf_start(ls_marker)
dw_1.Retrieve()
gnv_performance.uf_end(ls_marker)
...
...
ls_marker = 'nvo_my_obj.Constructor - SELECT ...'
gnv_performance.uf_start(ls_marker)
SELECT ... INTO ... FROM ...;
gnv_performance.uf_end(ls_marker)

If there are RETURNs between uf_start() and uf_end(), call uf_end() before each RETURN (and before a message box if it's shown).
So, one uf_start() will have multiple uf_end()'s, but only one must be executed.

***********************************************************************************************************************************************

HOW TO CHECK RESULT:

Open C:\performance_log.txt

The file name is defined in uf_get_log_file().

***********************************************************************************************************************************************

NESTED CALLS ARE ALLOWED:

uf_start() and uf_end() can ornament a code piece which calls another uf_start() + uf_end() pair.
Example: the total execution time of the constructor is captured with uf_start("Constructor") ... uf_end("Constructor").
The constructor calls uf_do_something(), which internally calls uf_start("uf_do_something") ... uf_end("uf_do_something") for its own measurement.
So, "uf_do_something" measurement is nested into "Constructor" measurement without interfering each other.

IMPORTANT! When markers are nested in a same script, don't use ls_marker variable:
           if the inner marker overrides it, a wrong value will be passed to uf_end() for the outer marker.

**********************************************************************************************************************************************/
end subroutine

public function integer uf_get_marker_index (string as_marker);int  li_index
int  li_upper_bound

li_upper_bound = UpperBound(is_markers)

for li_index = 1 to li_upper_bound
   if is_markers[li_index] = as_marker then
      return li_index
	end if
next

return 0
end function

public subroutine uf_start (string as_marker);int     i
boolean lb_called_first_time_for_this_marker
string  ls_msg

if as_marker = "" or IsNull(as_marker) then
	uf_log("You cannot pass empty marker to uf_start()")
	return
end if

i = uf_get_marker_index(as_marker)
lb_called_first_time_for_this_marker = (i = 0)

if lb_called_first_time_for_this_marker then
	// Add a new slot:
	i = UpperBound(is_markers) + 1
	is_markers[i] = as_marker
else
	// Don't add a new slot - we will re-use the existing one, just make sure uf_end() was called for it:
	if not IsNull(it_start_times[i]) then
		uf_log("~n~nYou call again:~n~ngnv_performance.uf_start('" + as_marker + "')~n~nBut you didn't close the previous uf_start() for this marker by uf_end()~n~n")
	end if
end if
it_start_times[i] = Now()
end subroutine

public subroutine uf_start_new_session ();string    ls_msg
WriteMode l_write_mode

ls_msg = "################# " + String(DateTime(Today(), Now()), "MMM dd - hh:mm:ss") + " #################~n "

if CLEAN_UP_PREV_SESSIONS then
	l_write_mode = Replace!
else
	l_write_mode = Append!
	ls_msg = "~n~n" + ls_msg
end if

uf_log(ls_msg, l_write_mode)
end subroutine

private function string uf_get_log_file ();return "C:\performance_log.txt"

// If it's a PowerServer app, you might want to include the user name (like "C:\performance_log_" + gs_user_name + ".txt")
// to allow multiple developers logging in the same web app without interfering each other.
end function

public subroutine uf_log (string as_msg, writemode a_write_mode);int li_file_no

li_file_no = FileOpen(uf_get_log_file(), LineMode!, Write!, Shared!, a_write_mode)
FileWrite(li_file_no, as_msg)
FileClose(li_file_no)
end subroutine

private subroutine uf_log (string as_msg);uf_log(as_msg, Append!)
end subroutine

on nvo_performance.create
call super::create
TriggerEvent( this, "constructor" )
end on

on nvo_performance.destroy
TriggerEvent( this, "destructor" )
call super::destroy
end on

