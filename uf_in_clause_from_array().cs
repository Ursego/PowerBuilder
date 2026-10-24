// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_in_clause_from_array() dynamically builds an IN clause (ready to be used in a WHERE clause) from the passed field name and array of values.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// I'll start with an example to show how the function works:

ls_statuses_frag = gn_util.uf_in_clause_from_array("status", {"A", "N", "R"}) // returns "status IN ('A', 'N', 'R')"
ls_depts_frag = gn_util.uf_in_clause_from_array("dept", {7, 11, 16}) // returns "dept IN (7, 11, 16)"
ls_sql = "SELECT " + as_fields_to_print_csv + " FROM emp WHERE " + ls_statuses_frag + " AND " + ls_depts_frag

// As you see in the first line, the string values are automatically ornamented with single quote marks, which is good for most databases.

// There are 2 versions - for MS SQL Server (which should work on Sybase ASE as well, and probably on most others) and for Oracle.
// Please chioose the one depending on your DB.

// Why a separate version for Oracle?
// Because Oracle allows a maximum of 1000 elements in an IN clause. Otherwise, you'll get the error "ORA-01795: Maximum number of expressions in a list is 1000".
// If the array contains more than 1000 elements, the function returns multiple IN clauses joined by ORs, something like:
// "(city IN ('elem1', ..., 'elem1000') OR city IN ('elem1001', ..., 'elem2000') OR city IN ('elem2001', ..., 'elem2800'))"

// Generally speaking, executing SQL statements on the client side is not good practice.
// Instead, the client should call a stored procedure that encapsulates any existing (or potential) complexity
//    and keeps the logic physically close to the data, minimizing unnecessary network traffic.
// However, if you have no choice - for example, when maintaining an application with a less-than-ideal architecture -
// then uf_in_clause_from_array() can make the code much clearer by keeping technical details out of the business logic.

// The function uses uf_replace_all(), so add it to your utilities NVO firstly: https://github.com/Ursego/PowerBuilder/blob/main/uf_replace_all().cs
// Or you can use another similar function which, probably, exists in your framework (like n_cst_string.of_GlobalReplace in PFC).

// So, here is the Oracle version:

/**********************************************************************************************************************
Dscr: Dynamically builds an IN clause ready to be used in a WHERE clause from the passed field name and array.

      Normally, you won't call this overload directly. Instead, call the dedicated overloads for STRING and LONG arrays:
      ls_statuses_frag = gn_util.uf_in_clause_from_array("status", {"A", "N", "R"}) // returns "status IN ('A', 'N', 'R')"
      ls_depts_frag = gn_util.uf_in_clause_from_array("dept", {7, 11, 16}) // returns "dept IN (7, 11, 16)"
      ls_sql = "SELECT " + as_fields_to_print_csv + " FROM emp WHERE " + ls_statuses_frag + " AND " + ls_depts_frag

      More info: https://github.com/Ursego/PowerBuilder/blob/main/uf_in_clause_from_array().cs
***********************************************************************************************************************
Arg:  as_field - to be placed just before the IN keyword
      as_arr - an array of STRING values to build the IN clause
      ab_field_is_textual - instructs if the values in the IN clause should be ornamented with single quotes:
			true - textual data type, use single quotes: "... IN ('123', '456')";
      	false - numeric data type, don't use single quotes: "... IN (123, 456)"
			Even when ab_field_is_textual = FALSE, the numeric values are passed through an array of strings (as_arr).
***********************************************************************************************************************
Ret:  String
***********************************************************************************************************************
Dev:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
char     			lc_quote
boolean 			 	lb_OR_used = false
boolean				lb_at_leat_one_element_added = false
int     			 	li_upper_bound
int      			i
int      			li_elements_counter = 1
string   			ls_result
string   			ls_element

if ab_field_is_textual then lc_quote = "'"

li_upper_bound = UpperBound(as_arr)
choose case li_upper_bound
case 0
   return "(1=0)" // produce FALSE where the IN clause would go
case 1
	if IsNull(as_arr[i]) or Trim(as_arr[i]) = '' then return "(1=0)"
   return as_field + " = " + lc_quote + as_arr[1] + lc_quote // produce an "equals" expression instead of a less efficient "IN"
end choose

ls_result = as_field + " IN ("
for i = 1 to li_upper_bound
   if IsNull(as_arr[i]) or Trim(as_arr[i]) = '' then continue
	ls_element = as_arr[i]
	uf_replace_all(ref ls_element, "'", "''") // https://github.com/Ursego/PowerBuilder/blob/main/uf_replace_all().cs
   // or using pfc_n_cst_string: ls_element = gnv_string.of_GlobalReplace(as_arr[i], "'", "''")
	ls_element = lc_quote + ls_element + lc_quote + ", "
	if Pos(ls_result, ls_element) > 0 then continue // it's already in the IN clause, adding it again makes no sense
	
   if li_elements_counter = 1000 /* prevent error "ORA-01795: Maximum number of expressions in a list is 1000" */ then
      li_elements_counter = 0
      ls_result = Left(ls_result, Len(ls_result) - 2) // remove the last comma and space (", ")
      ls_result += ") OR " + as_field + " IN ("
      lb_OR_used = true
   end if
	
   ls_result += ls_element
   li_elements_counter++
	lb_at_leat_one_element_added = true
next

if not lb_at_leat_one_element_added then return "(1=0)"

ls_result = Left(ls_result, Len(ls_result) - 2) // remove the last comma and space (", ")
ls_result += ")"

if lb_OR_used then
   ls_result = "(" + ls_result + ")" // ensure correct work of the IN clause when other conditions exist
end if

return ls_result

// And the MS SQL Server version:

/**********************************************************************************************************************
Dscr: Dynamically builds an IN clause ready to be used in a WHERE clause from the passed field name and array.

      Normally, you won't call this overload directly. Instead, call the dedicated overloads for STRING and LONG arrays:
      ls_statuses_frag = gn_util.uf_in_clause_from_array("status", {"A", "N", "R"}) // returns "status IN ('A', 'N', 'R')"
      ls_depts_frag = gn_util.uf_in_clause_from_array("dept", {7, 11, 16}) // returns "dept IN (7, 11, 16)"
      ls_sql = "SELECT " + as_fields_to_print_csv + " FROM emp WHERE " + ls_statuses_frag + " AND " + ls_depts_frag

      More info: https://github.com/Ursego/PowerBuilder/blob/main/uf_in_clause_from_array().cs
***********************************************************************************************************************
Arg:  as_field - to be placed just before the IN keyword
      as_arr - an array of STRING values to build the IN clause
      ab_field_is_textual - instructs if the values in the IN clause should be ornamented with single quotes:
			true - textual data type, use single quotes: "... IN ('123', '456')";
      	false - numeric data type, don't use single quotes: "... IN (123, 456)"
			Even when ab_field_is_textual = FALSE, the numeric values are passed through an array of strings (as_arr).
***********************************************************************************************************************
Ret:  String
***********************************************************************************************************************
Dev:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
char     			lc_quote
Boolean				lb_at_leat_one_element_added = false
int     			 	li_upper_bound
int      			i
int      			li_elements_counter = 1
string   			ls_result
string   			ls_element

if ab_field_is_textual then lc_quote = "'"

li_upper_bound = UpperBound(as_arr)
choose case li_upper_bound
case 0
   return "(1=0)" // produce FALSE where the IN clause would go
case 1
	if IsNull(as_arr[i]) or Trim(as_arr[i]) = '' then return "(1=0)"
   return as_field + " = " + lc_quote + as_arr[1] + lc_quote // produce an "equals" expression instead of a less efficient "IN"
end choose

ls_result = as_field + " IN ("
for i = 1 to li_upper_bound
   if IsNull(as_arr[i]) or Trim(as_arr[i]) = '' then continue
	ls_element = as_arr[i]
	uf_replace_all(ref ls_element, "'", "''") // https://github.com/Ursego/PowerBuilder/blob/main/uf_replace_all().cs
   // or using pfc_n_cst_string: ls_element = gnv_string.of_GlobalReplace(as_arr[i], "'", "''")
	ls_element = lc_quote + ls_element + lc_quote + ", "
	if Pos(ls_result, ls_element) > 0 then continue // it's already in the IN clause, adding it again makes no sense
	
   ls_result += ls_element
   li_elements_counter++
	lb_at_leat_one_element_added = true
next

if not lb_at_leat_one_element_added then return "(1=0)"

ls_result = Left(ls_result, Len(ls_result) - 2) // remove the last comma and space (", ")
ls_result += ")"

return ls_result

// Now, create some useful overloads (a same version for any DB):

// ### The overload to build an IN clause from a STRING array.
// It will exempt you from passing TRUE as the 3rd argument:

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

// ### The overload to build an IN clause from a LONG array.
// It will exempt you from converting your LONG array to a STRING array and passing FALSE as the 3rd argument:

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
string   ls_arr[]

li_upper_bound = UpperBound(al_arr)
for i = 1 to li_upper_bound
   ls_arr[i] = String(al_arr[i])
next

return uf_in_clause_from_array(as_field, ls_arr, false /* ab_data_is_textual */)
