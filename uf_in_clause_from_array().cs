// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_in_clause_from_array() dynamically builds an IN clause (ready to be used in a WHERE clause) from the passed field name and array of values.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// I will immediately give an example that shows how the function works:

ls_statuses_frag = gn_util.uf_in_clause_from_array("status", {"A", "N", "R"}) // returns "status IN ('A', 'N', 'R')"
ls_depts_frag = gn_util.uf_in_clause_from_array("dept", {7, 11, 16}) // returns "dept IN (7, 11, 16)"
ls_sql = "SELECT " + as_fields_to_print_csv + " FROM emp WHERE " + ls_statuses_frag + " AND " + ls_depts_frag

// As you see in the first line, the string values are automatically ornamented with single quote marks, which is good for most databases.

// Oracle only:
// If the array contains more than 1000 elements then the function returns a few IN clauses joined by OR to avoid Oracle error:
ORA-01795: Maximum number of expressions in a list is 1000
// For example, if the passed array contains 2800 elements then uf_in_clause_from_array() returns something like 
(last_name IN ('elem1', ..., 'elem1000') OR last_name IN ('elem1001', ..., 'elem2000') OR last_name IN ('elem2001', ..., 'elem2800'))"
// If your DB is not Oracle, you can remove that functionality from the function (or keep it - it will work fine for any DB).

// The function uses uf_replace_all() so add it to your utilities NVO: https://github.com/Ursego/PowerBuilder/blob/main/uf_replace_all().cs
// Or you can use another similar function which, probably, exists in your framework (like n_cst_string.of_GlobalReplace in PFC).

// Here is the function's code:

/**********************************************************************************************************************
Dscr:       Dynamically builds an IN clause (ready to be used in a WHERE clause) from the passed field name and array.
            To build an IN clause from an array of STRING, use the (string as_field, string as_arr[]) overload.
            To build an IN clause from an array of LONG, use the (string as_field, long al_arr[]) overload.
            Example:
            string ls_countries[] = {"Canada", "USA", "Australia"}
            ls_IN_clause = gn_util.uf_in_clause_from_array("country", ls_countries[]) // returns "country IN ('Canada', 'USA', 'Australia')"
***********************************************************************************************************************
Arg:        as_field - to be placed just before the IN keyword
            as_arr - array of STRING values to build IN clause
            ab_field_is_textual:
                true - textual data type, use single quotes: "... IN ('123', '456')";
                false - numeric data type, don't use single quotes: "... IN (123, 456)"
                Even when ab_field_is_textual = FALSE, the numeric values are passed through an array of strings (as_arr).
***********************************************************************************************************************
Ret:        string
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
char     lc_quote
boolean  lb_OR_used = false
int      li_upper_bound
int      i
int      li_elements_counter = 1
string   ls_result

if ab_field_is_textual then
   lc_quote = "'"
end if

li_upper_bound = UpperBound(as_arr)
choose case li_upper_bound
case 0
   return "(1=0)" // produce FALSE in the place where the IN clause would normally go
case 1
   return as_field + " = " + lc_quote + as_arr[1] + lc_quote // produce an "equals" expression instead of a less efficient "IN"
end choose

ls_result = as_field + " IN ("
for i = 1 to li_upper_bound
   if IsNull(as_arr[i]) or Trim(as_arr[i]) = '' then continue
   if li_elements_counter = 1000 /* prevent error "ORA-01795: Maximum number of expressions in a list is 1000" */ then
      li_elements_counter = 0
      ls_result = Left(ls_result, Len(ls_result) - 2) // remove the last comma and space (", ")
      ls_result += ") OR " + as_field + " IN ("
      lb_OR_used = true
   end if
   uf_replace_all(ref as_arr[i], "'", "''")
   ls_result += lc_quote + as_arr[i] + lc_quote + ", "
   li_elements_counter++
next
ls_result = Left(ls_result, Len(ls_result) - 2) // remove the last comma and space (", ")
ls_result += ")"

if lb_OR_used then
   ls_result = "(" + ls_result + ")" // ensure correct work of the IN clause when other conditions exist
end if

return ls_result

// Create an overloaded version without the ab_data_is_textual argument for cases where the field contains textual data
// (it will exempt you from providing TRUE as the 3rd argument):

/**********************************************************************************************************************
Dscr:       Dynamically builds an IN clause (ready to be used in a WHERE clause) from the passed field name and array of STRING.
            To build an IN clause from an array of LONG, use the (string as_field, long al_arr[]) overload.
***********************************************************************************************************************
Arg:        as_field - to be placed just before the IN keyword
            as_arr - an array of STRING values to build the IN clause
***********************************************************************************************************************
Ret:        string
***********************************************************************************************************************
Developer:  Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
return this.uf_in_clause_from_array(as_field, as_arr[], true /* ab_data_is_textual */)

// Finally, create one more overload for arrays of type LONG - this will be useful, for example, when generating IN clauses for numeric ID fields.

/**********************************************************************************************************************
Dscr:       Dynamically builds an IN clause (ready to be used in a WHERE clause) from the passed field name and array of LONG.
            To build an IN clause from an array of STRING, use the (string as_field, string as_arr[]) overload.
***********************************************************************************************************************
Arg:        as_field - to be placed just before the IN keyword
            al_arr - an array of LONG values to build the IN clause
***********************************************************************************************************************
Ret:        string
***********************************************************************************************************************
Developer:  Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
int      li_upper_bound
int      i
string   ls_IN_clause
string   ls_arr[]

li_upper_bound = UpperBound(al_arr)
for i = 1 to li_upper_bound
   ls_arr[i] = String(al_arr[i])
next

ls_IN_clause = uf_in_clause_from_array(as_field, ls_arr, false /* ab_data_is_textual */)

return ls_IN_clause

// Generally speaking, using SQL statements on the client side is not a good practice.
// The client should call a stored procedure that encapsulates any existing (or potential) complexity and keeps it on the server side.  
// But if you have no choice — for example, when maintaining an application where all the SQL is handled in PowerBuilder —
//      then uf_in_clause_from_array() can make your life easier, allowing you to avoid writing a loop.
