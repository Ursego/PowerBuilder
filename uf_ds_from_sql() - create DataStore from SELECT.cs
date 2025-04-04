// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The function returns a DataStore created dynamically by the supplied SELECT statement.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// It has two overloads.
// One accepts a Transaction object through the a_tr argument.
// Another overload doesn’t include that argument and uses SQLCA; it will likely be the most commonly used overload.

// The function takes a boolean argument ab_retrieve.
// If it's true, the function will not only create a DS but also populate it, so you can work with the retrieved data immediately after calling the function.
// I recommend creating constants for this argument in the same class where you create the function:
constant boolean RETRIEVE_DS = true
constant boolean DONT_RETRIEVE_DS = false

// More information and examples of use are provided in the header comment of the function.

// ERRORS HANDLING

// If an error occurs, the function displays an error message and returns a DataStore which has not been initialized, so the calling script must check it with IsValid().
// That is not the best way, but, probably, most developers will choose it since they don't use exceptions.

// The correct and preferable way is to use the exceptions mechanism: https://github.com/Ursego/PowerBuilder/blob/main/Exceptions%20-%20the%20elegant%20way.cs
// I will provide both variations, so you can choose the one you prefer.

// @@@ WITHOUT exceptions:

/**********************************************************************************************************************
Dscr:       Returns a DataStore created dynamically by the supplied SELECT statement.

            This func allows to AVOID:
               1. Flooding PBLs with single-use DataObjects whose visual representation is irrelevant.
               2. Complicated formats 3 and 4 of dynamic embedded SQL. It's much easier to build the SQL as a string,
                        pass it to uf_ds_from_sql() and then manipulate the retrieved data in the created DataStore.

            The DS cannot have retrieval arguments. Instead, add them to the WHERE clause of SELECT statement dynamically
            prior to passing it to uf_ds_from_sql(), for example:

            ls_sql = "SELECT " + as_list_of_fields_to_print + " FROM emp WHERE dept_id = " + String(al_dept_id)
            lds_emp = gn_util.uf_ds_from_sql(ls_sql)

            If an argument is an array, then you can create an IN clause using the function uf_in_clause_from_array():
            https://github.com/Ursego/PowerBuilder/blob/main/uf_in_clsause_from_array().cs
            It has two overloads - for arrays of types string and long:

            ls_statuses_frag = gn_util.uf_in_clause_from_array("status", {"A", "N", "R"}) // returns "status IN ('A', 'N', 'R')"
            ls_depts_frag = gn_util.uf_in_clause_from_array("dept", {7, 11, 16}) // returns "dept IN (7, 11, 16)"
            ls_sql = "SELECT " + as_list_of_fields_to_print + " FROM emp WHERE " + ls_statuses_frag + " AND " + ls_depts_frag
            lds_emp = gn_util.uf_ds_from_sql(ls_sql)
***********************************************************************************************************************
Arg:        as_sql: the SELECT statement to create the DS from (without ";").     
            ab_retrieve:
                  true = create a DS, and retrieve data immediately;
                  false = only create a DS, but don't retrieve.
            a_tr - Transaction object for created DS. To use SQLCA, call the overloaded version without this arg.
***********************************************************************************************************************
Ret:         DataStore. The calling script must check it with IsValid()!!!!
***********************************************************************************************************************
Developer:   Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long        ll_rc
string      ls_err = ''
string      ls_syntax
DataStore   lds

as_sql = Trim(as_sql)

choose case true
case as_sql = ''
   ls_err = "as_sql is empty string."
case IsNull(as_sql)
   ls_err = "as_sql is null."
case not IsValid(a_tr)
   ls_err = "a_tr is not valid."
end choose

if ls_err <> '' then
   ls_syntax = a_tr.SyntaxFromSQL(as_sql, "style(type=grid)", ref ls_err)
   if Len(ls_err) > 0 then ls_err = "SyntaxFromSQL() failed:~r~n~r~n" + ls_err + "."
end if

if ls_err <> '' then
   lds = create DataStore
   lds.Create(ls_syntax, ref ls_err)
   if Len(ls_err) > 0 then ls_err = "Create() failed:~r~n~r~n" + ls_err + "."
end if

if ls_err <> '' then
   ll_rc = lds.SetTransObject(a_tr)
   if ll_rc = -1 then ls_err = "SetTransObject() failed."
end if

if ls_err <> '' and ab_retrieve then
   ll_rc = lds.Retrieve()
   if ll_rc = -1 then ls_err = "Retrieve() failed."
end if

if ls_err <> '' then
   if IsValid(a_tr) then ls_err += "~r~n~r~nServerName = '" + a_tr.ServerName + "'; LogID = '" + a_tr.LogID + "'."
   if Len(as_sql) > 0 then ls_err += "~r~n~r~nSQL SELECT:~r~n~r~n" + as_sql
   MessageBox("Error in uf_ds_from_sql()", ls_err)
end if

return lds

// The overload which uses SQLCA; it has only 2 arguments - as_sql and ab_retrieve, but not a_tr:

return this.uf_ds_from_sql(as_sql, ab_retrieve, SQLCA)

// The overload which uses SQLCA and retrieves; it has only 1 argument - as_sql, but not ab_retrieve and a_tr:

return this.uf_ds_from_sql(as_sql, RETRIEVE_DS)

// @@@ WITH exceptions:

// Obviously, first of all you need to add the exceptions mechanism to your app:
// https://github.com/Ursego/PowerBuilder/blob/main/Exceptions%20-%20the%20elegant%20way.cs

// Don't forget to fill the "Throws:" field in the header with n_ex in both the overloads!

/**********************************************************************************************************************
Dscr:       Returns a DataStore created dynamically by the supplied SELECT statement.

            This func allows to AVOID:
               1. Flooding PBLs with single-use DataObjects whose visual representation is irrelevant.
               2. Complicated formats 3 and 4 of dynamic embedded SQL. It's much easier to build the SQL as a string,
                        pass it to uf_ds_from_sql() and then manipulate the retrieved data in the created DataStore.

            The DS cannot have retrieval arguments. Instead, add them to the WHERE clause of SELECT statement dynamically
            prior to passing it to uf_ds_from_sql(), for example:

            ls_sql = "SELECT " + as_list_of_fields_to_print + " FROM emp WHERE dept_id = " + String(al_dept_id)
            lds_emp = gn_util.uf_ds_from_sql(ls_sql)

            If an argument is an array, then you can create an IN clause using the function uf_in_clause_from_array():
            https://github.com/Ursego/PowerBuilder/blob/main/uf_in_clause_from_array().cs
            It has two overloads - for arrays of types string and long:

            ls_statuses_frag = gn_util.uf_in_clause_from_array("status", {"A", "N", "R"}) // returns "status IN ('A', 'N', 'R')"
            ls_depts_frag = gn_util.uf_in_clause_from_array("dept", {7, 11, 16}) // returns "dept IN (7, 11, 16)"
            ls_sql = "SELECT " + as_list_of_fields_to_print + " FROM emp WHERE " + ls_statuses_frag + " AND " + ls_depts_frag
            lds_emp = gn_util.uf_ds_from_sql(ls_sql)
***********************************************************************************************************************
Arg:        as_sql: the SELECT statement to create the DS from (without ";").     
            ab_retrieve:
                  true = create a DS, and retrieve data immediately;
                  false = only create a DS, but don't retrieve.
            a_tr - Transaction object for created DS. To use SQLCA, call the overloaded version without this arg.
***********************************************************************************************************************
Ret:        DataStore
***********************************************************************************************************************
Throws:     n_ex
***********************************************************************************************************************
Developer:  Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long        ll_rc
string      ls_err
string      ls_syntax
DataStore   lds

as_sql = Trim(as_sql)

try
   if as_sql = '' then f_throw(PopulateError(1, "as_sql is empty string.")) // https://github.com/Ursego/PowerBuilder/blob/main/uf_in_clause_from_array().cs
   if IsNull(as_sql) then f_throw(PopulateError(2, "as_sql is null."))
   if not IsValid(a_tr) then f_throw(PopulateError(3, "a_tr is not valid."))

   ls_syntax = a_tr.SyntaxFromSQL(as_sql, "style(type=grid)", ref ls_err)
   if Len(ls_err) > 0 then f_throw(PopulateError(4, "SyntaxFromSQL() failed:~r~n~r~n" + ls_err + "."))
   
   lds = create DataStore
   lds.Create(ls_syntax, ref ls_err)
   if Len(ls_err) > 0 then f_throw(PopulateError(5, "Create() failed:~r~n~r~n" + ls_err + "."))
   
   ll_rc = lds.SetTransObject(a_tr)
   if ll_rc = -1 then f_throw(PopulateError(6, "SetTransObject() failed."))
   
   if ab_retrieve then
      ll_rc = lds.Retrieve()
      if ll_rc = -1 then f_throw(PopulateError(7, "Retrieve() failed."))
   end if
catch (n_ex ln_ex)
   if IsValid(a_tr) then ls_err += "~r~n~r~nServerName = '" + a_tr.ServerName + "'; LogID = '" + a_tr.LogID + "'."
   if Len(as_sql) > 0 then ls_err += "~r~n~r~nSQL SELECT:~r~n~r~n" + as_sql
   ln_ex.SetMessage(ls_err)
   throw ln_ex
end try

return lds

// The overloads use the same code as the exceptionless overloads — just make them throw n_ex.

// Generally speaking, using SQL statements on the client side is not a good practice.
// The client should call a stored procedure that encapsulates any existing (or potential) complexity and keeps it on the server side.  
// But if you have no choice — for example, when maintaining an application where all the SQL is handled in PowerBuilder —
//      then uf_ds_from_sql() avoids creation of a large number of DataObjects, used only once.
