// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Use uf_filter() provided below instead of the DW's built-in functions SetFilter() and Filter().
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Create uf_filter() in the ancestor DW of your application (and DS, if you need).

// That solution:

// 1. Makes the calling script shorter. For example, the fragment

dw_salary.SetFilter(ls_filter_expr)
dw_salary.Filter()
dw_salary.GroupCalc()
ll_row_count = dw_salary.RowCount()

// shrinks to one line:

ll_row_count = dw_salary.uf_filter(ls_filter_expr)

// 2. Alerts if the passed expression is NULL, which can happen when the expression is built dynamically and a variable with a NULL value is concatenated.
// An empty string is not treated as an error, as it is used to remove filtering.

// 3. If the search expression is incorrect, it shows a programmer-friendly message that:  
//      A. Displays the invalid expression.  
//      B. Shows the DataWindow's DataObject and suggests checking whether all referenced fields exist in it.  

// Before adding uf_filter(), create the Exceptions functionality (https://github.com/Ursego/DWSpy).

// Here is the source of the function:

/**********************************************************************************************************************
Dscr:       Filters the DW by the passed expression.
***********************************************************************************************************************
Arg:        as_new_filter
***********************************************************************************************************************
Ret:        long (number of rows in Primary! buffer after filtering)
***********************************************************************************************************************
Throws:     n_ex
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
int li_rc

if IsNull(as_new_filter) then f_throw(PopulateError(1, "Filter exression is NULL."))

li_rc = this.SetFilter(as_new_filter)
if li_rc <> 1 then f_throw(PopulateError(2, "this.SetFilter() failed.~r~n~r~nFilter Expression:~r~n" + as_new_filter + &
                                             "~r~n~r~nCheck if all fields, mentioned in expression, exist in DataObject '" + this.DataObject + "'."))
li_rc = this.Filter()
if li_rc <> 1 then f_throw(PopulateError(3, "this.Filter() failed.~r~n~r~nFilter Expression:~r~n" + as_new_filter + &
                                             "~r~n~r~nCheck if all fields, mentioned in expression, exist in DataObject '" + this.DataObject + "'."))

this.GroupCalc()

return this.RowCount()

// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_append_filter()
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// An additional function which appends a new condition to the existing filter using the AND logic.
// Create it in your ancestor DW/DS as well:

/**********************************************************************************************************************
Dscr:       Works as uf_filter() but new filter is added to existing instead of replacing it.
***********************************************************************************************************************
Arg:        as_filter_to_append
***********************************************************************************************************************
Ret:        long (number of rows in Primary! buffer after filtering)
***********************************************************************************************************************
Throws:     n_ex
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
string ls_existing_filter
string ls_new_filter

ls_existing_filter = this.object.datawindow.table.filter
choose case ls_existing_filter
case "?", "!"
   ls_new_filter = as_filter_to_append
case else
   ls_new_filter = "(" + ls_existing_filter + ") AND (" + as_filter_to_append + ")"
end choose

return uf_filter(ls_new_filter)