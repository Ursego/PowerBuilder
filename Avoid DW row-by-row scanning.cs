// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Don't process DataWindows and DataStores row by row if the task can be accomplished in another, more efficient way.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// @@@ Evaluate()

// In many situations (some of which are described below), the function
Describe("Evaluate('<EXPRESSION>, 1, 0) for all', 1)")
// is very useful if you want to avoid scanning the DW in a loop with a comparison in each row.

// Count rows which satisfy a criteria:

ll_active_emp_count = Long(dw_emp.Describe("Evaluate('Sum(if(status = ~"A~", 1, 0) for all)', 1)"))
MessageBox("HR", "There are " + String(ll_active_emp_count) + " active employees.")

// Maximum value of the field in all rows:

ld_latest_order_date = Date(ids_order.Describe("Evaluate('Max(order_date)', 0)"))
MessageBox("Orders", "The last order was made in " + String(ld_latest_order_date, '"MMM DD, YYYY") + ".")

// Count selected (highlighted) rows:

ll_selected_count = Long(dw_emp.Describe("Evaluate('Sum(if(IsSelected(), 1, 0) for all)', 1)"))
if ll_selected_count < 2 then MessageBox("New Team", "Please select at least 2 employees.")

// Define if the field has a duplicated value (i.e. is not unique):

dw_order_status.SetSort('order_status A')
dw_order_status.Sort()
dw_order_status.GroupCalc()
lb_duplicate_exists = ("1" = dw_order_status.Describe("Evaluate('Max(if(GetRow() <> 1 AND order_status = order_status[-1], 1, 0))', 0)"))
if lb_duplicate_exists then MessageBox("Error", "Order Statuses must be unique.")

// Populate a field in all the rows with a same value:

// Sometimes, we need to assign the same value to a field across all rows.  
// For example, consider a coefficient that another field should be multiplied by.
// This coefficient is calculated at runtime and therefore cannot be hardcoded in a computed column or field.  
// Some developers handle this by adding a NULL column to the DataWindow's SELECT statement and looping through all rows to assign the value manually.  
// But there's a better way: make the field a computed field with "1" as its expression, and then programmatically update the expression by setting the value as the new expression.

// *** BAD code: ***

// Suppose a variable ll_coef_to_divide contains the result of a calculation in PowerScript.  
// Here’s the inefficient solution (assuming the field is not computed and exists in the DataWindow's data source):

for ll_row = 1 to ll_row_count
       dw_XXX.object.coef_to_divide[ll_row] = ll_coef_to_divide
next

// *** GOOD code: ***

// To make the assignment at one stroke, it must be a computed field. The value that the field will return is assigned as follows:

dw_XXX.object.c_coef_to_divide.Expression = String(ll_coef_to_divide)
dw_XXX.GroupCalc() // recalc other computed fields - c_coef_to_divide can appear in their expressions (of course, converted from string to decimal)

// @@@ Find and process rows which satisfy a criteria

// Instead of looping on the whole DW and checking each row, use a search expression passed to Find():

// *** BAD code: ***

ll_row_count = dw_emp.RowCount()
for ll_row = 1 to ll_row_count
       ll_curr_dept_id = dw_emp.object.dept_id[ll_row]
       if ll_curr_dept_id <> al_dept_id then continue
       // ...process the found row...
next

// *** GOOD code: ***

ll_row = 0
ll_row_count = dw_emp.RowCount()
ls_search_expr = "dept_id=" + String(al_dept_id)
do while true
       ll_row = dw_emp.Find(ls_search_expr, ll_row + 1, ll_row_count)
       if ll_row = 0 then exit
       // ...process the found row...
       if ll_row = ll_row_count then exit // prevent an eternal loop when the last row satisfies the search condition
loop


// Even though we are still looping, the number of iterations will be much smaller — possibly just a few or even one, instead of hundreds.
