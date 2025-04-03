// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Do not trust DWItemStatus. Instead of GetItemStatus, use uf_col_modified() and uf_row_modified(), suggested below.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Checking the field's DWItemStatus isn't always reliable: if the value was changed and then reverted to its original value, the column is still marked as DataModified!.
// It's a heavy bug of PB! If the value was changed but later restored to its original state, the column must not be considered modified.
// The only dependable way to determine whether a value has really changed is by comparing the current and original values - keeping in mind that either or both could be NULL.
// And that is exactly what uf_col_modified() is doing.

// @@@ uf_col_modified()

/**********************************************************************************************************************
Dscr:       Reports if the passed column has been modified, i.e. its value has been changed since the row was
            retrieved or lastly saved. If the value was changed, but later the original value
            was restored, the field is NOT considered modified regardless of its DWItemStatus.
           
            If your script deals only with the current record of the Primary! buffer (i.e. it's a FORM DW),
            then you can use the overloaded version with 2 arguments (adw & as_col).
***********************************************************************************************************************
Arg:        DataWindow   adw
            long         al_row
            string       as_col
            DWBuffer     a_buf
***********************************************************************************************************************
Ret:        boolean (true - has been changed, false - has NOT been changed)
***********************************************************************************************************************
Developer:  Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego/PowerBuilder
**********************************************************************************************************************/
string         ls_col_type
string         ls_old
string         ls_new
DWItemStatus   l_col_status

l_col_status = adw.GetItemStatus(al_row, as_col, a_buf)
if l_col_status = NotModified! then
   return false
end if

ls_col_type = Lower(Left(adw.Describe(as_col + ".coltype"), 5))
choose case ls_col_type
case "numbe", "long", "ulong", "real", "int", "decim"
   ls_old = String(adw.GetItemNumber(al_row, as_col, a_buf, true))
   ls_new = String(adw.GetItemNumber(al_row, as_col, a_buf, false))
case "char(", "char"
   ls_old = adw.GetItemString(al_row, as_col, a_buf, true)
   ls_new = adw.GetItemString(al_row, as_col, a_buf, false)
case "datet", "times"
   ls_old = String(adw.GetItemDateTime(al_row, as_col, a_buf, true))
   ls_new = String(adw.GetItemDateTime(al_row, as_col, a_buf, false))
case "date"
   ls_old = String(adw.GetItemDate(al_row, as_col, a_buf, true))
   ls_new = String(adw.GetItemDate(al_row, as_col, a_buf, false))
case "time"
   ls_old = String(adw.GetItemTime(al_row, as_col, a_buf, true))
   ls_new = String(adw.GetItemTime(al_row, as_col, a_buf, false))
end choose

if IsNull(ls_old) and IsNull(ls_new) then return false
if IsNull(ls_old) and not IsNull(ls_new) then return true
if not IsNull(ls_old) and IsNull(ls_new) then return true

return (ls_new <> ls_old)

// Many times, the script works only with the current row in the Primary! buffer (for example, in a form DW, or in a multi-row DW without filtering or deletions).  
// To handle that case, create an overloaded version with only two arguments — adw and as_col_name — and the following script:

return uf_col_modified(adw, as_col, adw.GetRow(), Primary!)

// @@@ uf_row_modified()

// Reports if the passed row has been modified, i.e. at least one of its columns has been changed. 

/**********************************************************************************************************************
Dscr:       Reports if the passed row has been modified, i.e. at least one of its columns has been changed.
            A column is considered modified if its value has been changed since the row was retrieved or lastly saved.
            If the value was changed, but later the original value was restored,
            the column is NOT considered modified regardless of its DWItemStatus.
           
            If your script deals only with the current record of the Primary! buffer (i.e. it's a form DW),
            then you can use the overloaded version with 1 argument only (adw).
***********************************************************************************************************************
Arg:        adw            DataWindow
            al_row         long
            DWBuffer       a_buf
***********************************************************************************************************************
Ret:        boolean
***********************************************************************************************************************
Developer:  Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego/PowerBuilder
**********************************************************************************************************************/
int            i
int            li_col_count
string         as_col
DWItemStatus   l_row_status

l_row_status = adw.GetItemStatus(al_row, 0, a_buf)
choose case l_row_status
case New!, NotModified!
   return false
end choose

li_col_count = Integer(adw.Describe("datawindow.column.count"))
for i = 1 to li_col_count
   as_col = adw.Describe("#"+ String(i) + ".Name")
   if uf_col_modified(adw, as_col, al_row, a_buf) then
      return true
   end if
next

return false

// The code of an overload to check only the current row in the Primary! buffer:

return uf_row_modified(adw, adw.GetRow(), Primary!)