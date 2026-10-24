// Here is a simple INI file manager that you can use instead of the standard PowerBuilder functions GetProfileString() and GetProfileInt().

// Examples of use:

is_my_var = gnv_ini.uf_get_string('MySection', 'MyStringValue')
ib_my_var = gnv_ini.uf_get_boolean('MySection', 'MyBooleanValue')
ii_my_var = gnv_ini.uf_get_int('MySection', 'MyIntValue')
il_my_var = gnv_ini.uf_get_long('MySection', 'MyLongValue')

// The manager provides the following features:

// * Easy reading of Long INI properties. You do not need to call GetProfileString() and then convert the result to Long.
// * Easy reading of Boolean INI properties. The values '1', 'true', 'y', and 'yes' are treated as TRUE (case-insensitive). Any other value is treated as FALSE.
// * If the value of an Int or Long INI property is not numeric or is outside the valid range for the data type, a clear error message describing the problem is displayed.
// * If a property is not found in the INI file, a clear error message describing the problem is displayed.
//      REMARK:
//      All properties are considered mandatory. They may have empty values (like MyProperty=), but they must be present in the INI file.
//      The practice of supplying a default value to GetProfileString() or GetProfileInt() goes against the principles of defensive programming.
//      Consider a real-world example: while editing the INI file, someone accidentally deletes a property.
//      Another example: a developer adds a new INI property on their local DEV machine but forgets to copy the updated INI file to PROD.
//      In these cases, we want a hard error rather than silent fallback behavior that can hide a bug.

// Steps to create:

// @@@
// Declare global var:
string	gs_app_directory

// @@@
// Add to the Application object's Open event:
gs_app_directory = GetCurrentDirectory()

// @@@
// Create f_empty() global function which returns Boolean and accepts as_value String argument. The code:
if IsNull(as_value) then return true
if Trim(as_value) = '' then return true
return false

// @@@
// Create f_throw() global function which returns (None), accepts as_msg String argument and thows Exception. The code:
Exception	l_ex
l_ex = create Exception
l_ex.SetMessage(as_msg)
throw l_ex

// @@@
// Create n_ini.sru text file with the following code:
$PBExportHeader$n_ini.sru
forward
global type n_ini from nonvisualobject
end type
end forward

global type n_ini from nonvisualobject
end type
global n_ini n_ini

type variables
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// n_ini is a simple INI file manager that you can use instead of the standard PowerBuilder functions GetProfileString() and GetProfileInt().
//
// Examples of use:
//
// is_my_var = gnv_ini.uf_get_string('MySection', 'MyStringValue')
// ib_my_var = gnv_ini.uf_get_boolean('MySection', 'MyBooleanValue')
// ii_my_var = gnv_ini.uf_get_int('MySection', 'MyIntValue')
// il_my_var = gnv_ini.uf_get_long('MySection', 'MyLongValue')
//
// The manager provides the following features:
//
// * Easy reading of Long INI properties. You do not need to call GetProfileString() and then convert the result to Long.
// * Easy reading of Boolean INI properties. The values '1', 'true', 'y', and 'yes' are treated as TRUE (case-insensitive). Any other value is treated as FALSE.
// * If the value of an Int or Long INI property is not numeric or is outside the valid range for the data type, a clear error message describing the problem is displayed.
// * If a property is not found in the INI file, a clear error message describing the problem is displayed.
//      REMARK:
//      All properties are considered mandatory. They may have empty values (like MyProperty=), but they must be present in the INI file.
//      The practice of supplying a default value to GetProfileString() or GetProfileInt() goes against the principles of defensive programming.
//      Consider a real-world example: while editing the INI file, someone accidentally deletes a property.
//      Another example: a developer adds a new INI property on their local DEV machine but forgets to copy the updated INI file to PROD.
//      In these cases, we want a hard error rather than silent fallback behavior that can hide a bug.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
end variables
forward prototypes
public function string uf_get_string (string as_section, string as_param) throws exception
public function long uf_get_long (string as_section, string as_param) throws exception
public function boolean uf_get_boolean (string as_section, string as_param) throws exception
public function string uf_get_ini_file ()
public function integer uf_get_int (string as_section, string as_param) throws exception
end prototypes

public function string uf_get_string (string as_section, string as_param) throws exception;string	ls_value, ls_ini_file

constant string PROPERTY_NOT_FOUND = '---===###PROPERTY_NOT_FOUND###===---'

ls_ini_file = uf_get_ini_file()
 
ls_value = ProfileString(ls_ini_file, as_section, as_param, PROPERTY_NOT_FOUND)
if ls_value = PROPERTY_NOT_FOUND then
	if f_empty(as_param) then f_throw('n_ini.uf_get_string: as_param parameter must not be empty.')
	if f_empty(as_section) then f_throw('n_ini.uf_get_string: as_section parameter must not be empty.')
	f_throw('n_ini.uf_get_string: failed to read "' + as_param + '" parameter from "' + as_section + '" section of "' + ls_ini_file + '" file.')
end if

return ls_value
end function

public function long uf_get_long (string as_section, string as_param) throws exception;string	ls_value
long		ll_value

ls_value = uf_get_string(as_section, as_param)

try
	ll_value = Long(ls_value)
catch (RuntimeError re)
	f_throw('n_ini.uf_get_long: parameter "' + as_param + '" in "' + as_section + '" section of "' + uf_get_ini_file() + '" file does not contain a valid number, or is outside the Long range: ' + ls_value)
end try

return ll_value
end function

public function boolean uf_get_boolean (string as_section, string as_param) throws exception;string	ls_value

ls_value = uf_get_string(as_section, as_param)

choose case Lower(ls_value)
case '1', 'true', 'y', 'yes'
	return true
case else
	return false
end choose
end function

public function string uf_get_ini_file ();return gs_app_directory + '\comwave.ini'
end function

public function integer uf_get_int (string as_section, string as_param) throws exception;string	ls_value
int		li_value

ls_value = uf_get_string(as_section, as_param)

try
	li_value = Integer(ls_value)
catch (RuntimeError re)
	f_throw('n_ini.uf_get_int: parameter "' + as_param + '" in "' + as_section + '" section of "' + uf_get_ini_file() + '" file does not contain a valid number, or is outside the Integer range: ' + ls_value)
end try

return li_value
end function

on n_ini.create
call super::create
TriggerEvent( this, "constructor" )
end on

on n_ini.destroy
TriggerEvent( this, "destructor" )
call super::destroy
end on

// @@@
// Import n_ini.sru to one of your PBLs.

// @@@
// In the uf_get_ini_file() function, change the name of the INI file to yours:
return gs_app_directory + '\<SUBSTITUTE WITH YOURS>.ini'

// @@@
// Declare global var:
n_ini		gnv_ini

// @@@
// Add to the Application object's Open event:
gnv_ini = create n_ini