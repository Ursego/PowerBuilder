// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_row_exists() reports if the DW has a row which satisfies the passed search expression.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// This function should be used for DW row existence check instead of the built-in DW's Find() function. Advantages:

// 1. Decreases the number of code lines because the calling script doesn't need to declare and then check the row number var. So, the fragment

long ll_row

ll_row = dw_test.Find(as_search_expr, 1, dw_test.RowCount())
if ll_row > 0 then...

// shrinks to one line:

if dw_test.uf_row_exists(as_search_expr) then...

// 2. If the search expression is wrong, the standard Find() function displays the message "Expression is not valid," which only makes us frustrated - which expression, where? 
// Why the hell doesn't Find() indicate the failed script, nor does it show the invalid expression that could help us locate that script?

// In contrast to Find(), uf_row_exists() clearly explains what went wrong by showing the invalid expression.
// That can help us to find the script. For example, if the expression is
emp_id=12345OR emp_role=3
// then we can globally search for the string
uf_row_exists("emp_id=
// to find the script which called the failed uf_row_exists(), and figure out why there is no space before the OR.
// If the actual parameter passed to uf_row_exists() was a variable, this trick will not work, but we still can search by fragments of the expression which are probably static.
// For example, a search for the next string can lead us to the goal, even if a few spots found:
OR emp_role=

// The error message from uf_row_exists() also displays the DataObject's name and suggests checking whether it includes all the fields mentioned in the expression.  
// You might have forgotten to add a new field to the DataObject, and in that case, you can simply go and add it.

// So, uf_row_exists() addresses the two most common reasons why Find() fails:
//      A malformed expression (like "emp_id=12345OR emp_role=3").
//      A non-existing field in the expression (like "emp_id=12345 OR mp_role=3").

// Before adding uf_show_columns(), create the Exceptions functionality (https://github.com/Ursego/DWSpy).
// This way you will create the f_throw() and IfNull() functions that uf_row_exists() calls.

// Create the function in the base DW ancestor:

/**********************************************************************************************************************
Dscr:       Reports if the DW has a row which satisfies the passed search expression.
***********************************************************************************************************************
Arg:        as_search_expr - the logical expression to search by
***********************************************************************************************************************
Ret:        boolean
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long   ll_row_count
long   ll_row

ll_row_count = this.RowCount()
if ll_row_count = 0 then return false

as_search_expr = Trim(as_search_expr)

choose case true
case IfNull(as_search_expr, '') = ''
   f_throw(PopulateError(1, "Arg as_search_expr is empty."))
case this.DataObject = ''
   f_throw(PopulateError(2, "DW has no DataObject."))
end choose

ll_row = this.Find(as_search_expr, 1, ll_row_count)
choose case ll_row
case is > 0
   return true
case 0
   return false
case -1 // general error
   f_throw(PopulateError(3, "Find() returned -1 (general error)."))
case -5 // bad argument
   f_throw(PopulateError(4, "Invalid search expression:~r~n~r~n~"" + as_search_expr + &
                  "~"~r~n~r~nCheck if all the mentioned fields exist in " + this.DataObject + "."))
end choose

// The function can be also created in the DataStore's ancestor and for DataWindowChild.
// For DataWindowChild, it should be added to another object - for example, the DW's ancestor or the util NVO - and accept the DWC as a parameter).