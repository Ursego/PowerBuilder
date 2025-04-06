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

// 2. If the search expression contains a bug, the standard Find() function displays the message "Expression is not valid," which only makes us frustrated. 
// It doesn't indicate which script caused the error, nor does it show the invalid expression.

// In contrast to Find(), uf_row_exists() clearly explains what went wrong and where.  
// Thanks to the use of the exceptions mechanism, the error message provides the exact object, script, and even the line number.
// You’ll also see the invalid expression passed to uf_row_exists().

// The error message from uf_row_exists() also displays the DataObject's name and suggests checking whether it includes all the fields mentioned in the expression.  
// You might have forgotten to add a new field to the DataObject, and in that case, you can simply go and add it.

// So, uf_row_exists() addresses the two most common reasons why Find() fails:
//      A malformed expression (like "emp_id=12345OR emp_role=3").
//      A non-existing field in the expression (like "emp_id=12345 OR emp_rle=3").

// Before adding uf_show_columns(), create the Exceptions functionality (https://github.com/Ursego/DWSpy).
// This way you will create the f_throw() and IfNull() functions that uf_row_exists() calls.

// Create the function in the base DW ancestor:

/**********************************************************************************************************************
Dscr:       Reports if the DW has a row which satisfies the passed search expression.
            More details: http://code.intfast.ca/viewtopic.php?f=4&t=83
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