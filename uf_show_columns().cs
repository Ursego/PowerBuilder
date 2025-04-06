// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_show_columns() shows only certain columns in the DW
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// The function accepts an array of DW column names and makes them visible, and hides the rest of the columns (all those which are not passed).
// It also shows or hides the corresponding labels, but they must follow the <sol name>_t naming convention in Tabular and FreeForm DWs.
// Columns in a Grid DW will be shown in the order they appear in as_columns[].

// Example of use:

gn_util.uf_show_columns(dw_dept, as_cols_to_show[])
gn_util.uf_show_columns(dw_emp, {"emp_id", "list_name", "first_name"})

// This function is useful in situations where different sets of fields need to be shown depending on circumstances that will only be known at runtime.
// First fill the array according to the conditions, and then call the function once passing the array.

// Before adding uf_show_columns(), create the Exceptions functionality (https://github.com/Ursego/DWSpy).
// This way you will create the f_throw() and IfNull() functions that uf_show_columns() uses.

// The source code:

/**********************************************************************************************************************
Dscr:       Shows (makes visible) the passed columns and hides (makes invisible) the rest of the columns in the DW.
            Columns in a Grid DW will be shown in the order they appear in as_columns[].
            The function also shows or hides the corresponding labels,
                but they must follow the <sol name>_t naming convention in Tabular and FreeForm DWs.
***********************************************************************************************************************
Arg:        adw - DataWindow
            as_columns[] - names of columns to show
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
int      li_count
int      i
string   ls_col_name
string   ls_col_idx
string   ls_rc

adw.SetRedraw(false)
adw.post SetRedraw(true)

// Step 1: hide ALL columns:
li_count = Integer(adw.Describe("DataWindow.Column.Count"))
for i = 1 to li_count
   ls_col_idx = "#"+ String(i)
   ls_col_name = Upper(adw.Describe(ls_col_idx + ".Name"))
   adw.Modify(ls_col_name + ".Visible = 0") // don't check ret code - the col exists for sure since it was returned by Describe()
   adw.Modify(ls_col_name + "_t.Visible = 0") // don't check ret code - existence of the "..._t" label is not guaranteed
next
   
// Step 2: show the columns passed in as_columns[]:
li_count = UpperBound(as_columns[])
for i = 1 to li_count
   ls_rc = adw.Modify(as_columns[i] + ".Visible = 1")
   if ls_rc = "!" then
      f_throw(PopulateError(0, "Column '" + IfNull(as_columns[i], "<NULL>") + &
                              "', passed in as_columns[" + String(i) + "], doesn't exist in " + adw.DataObject + "."))
   end if
   adw.Modify(as_columns[i] + "_t.Visible = 1") // don't check ret code - existence of the "..._t" label is not guaranteed
next

return

// There are two reason why the passed cols are processed in a separate loop while the first loop seems to be able to do the whole work:
//      Trowing an exception if the developer passed a wrong column name.
//      Showing the columns in a Grid DW in the order they are passed.