// To make a DataWindow column non-editable, many developers mark the column as Protected (or use an expression for conditional protection) in the DataWindow Painter.
// However, this is considered poor practice — I recommend using the function `uf_protect_col()`, whose code is provided below.

// The function has four overloads which protect:
//     * a single column either conditionally or unconditionally, and
//     * multiple columns at once (passed as an array), also conditionally or unconditionally.

// Call `uf_protect_col()` in the script that initializes your object — for example, in the `Open` event of a window or the constructor of a visual UserObject.
// If you're using a framework, you likely have an event such as `ue_post_open` or `ue_post_constructor`, which is triggered after the object has been constructed.

// If later you want to remove protection unconditionally, simply pass the expression `"1=0"`.

// @@@ Advantages

// Using `uf_protect_col()` instead of setting the Protect property in the DataWindow Painter offers a few benefits:

// @ You have one centralized script to manage fields editability. By reviewing it, you get a clear overview of which columns are protected and under what conditions.
// When using the DataWindow Painter, this information is hidden — you have to check each column’s properties individually.
// Of course, you can protect fields in any script at any time, usually using an unconditional overload of uf_protect_col().
// For example, right after data retrieval, or in the ItemChanged event. However, this should generally be avoided.
// It's best to manage the protection of all columns within a single script — this makes the overall logic easier to understand and maintain, and less error-prone.
// Only apply protection from other scripts when absolutely necessary.

// @ You can construct complex protection expressions dynamically and assign them in a cleaner way than with `Modify()`.

// @ Since the protection expressions are formed in PowerScript, it's easy to use constants instead of hardcoding values.

// @ If multiple columns share the same expression, you can built it once and apply it to all the columns, avoiding code duplication:

string ls_protect_expr
string ls_credit_card_cols[] = {"cc_holder_name", "cc_number", "cc_exp_month", "cc_exp_year", "cc_control_number"}

ls_protect_expr = "payment_method <> '" + n_payment_method.CREDIT_CARD + "'"
gnv_util.uf_protect_col(this, ls_credit_card_cols[], ls_protect_expr)

// @ When hardcoding expressions in the Protect property, you're limited to accessing only DataWindow columns and global functions (built-in or user).
// In contrast, uf_protect_col() allows you to use Boolean expressions built in PowerScript, giving you full flexibility.
// For example, you can call object methods, not just global functions.
// These methods can query the database without performance issues, since they are called only once when your script is executed.
// On the other hand, if you perform a database query inside a global function used in a hardcoded expression, that query will be executed every time the expression is evaluated.
// To prevent performance issues, you will be forced to implement caching.
// When the condition is defined in PowerScript (rather than in the Protect expression), an unconditional overload of uf_protect_col() is used, usually called in an "if" block:

string ls_credit_card_cols[] = {"cc_holder_name", "cc_number", "cc_exp_month", "cc_exp_year", "cc_control_number"}

if dw_billing.uf_get_payment_method() <> n_payment_method.CREDIT_CARD then
   gnv_util.uf_protect_col(this, ls_credit_card_cols[])
end if

// @@@ Source code

// Here is the source code for uf_protect_col(). Add it to your utilities NVO and pass the DataWindow as the 'adw' argument.
// If you prefer to add the function to your ancestor DataWindow, remove the 'adw' argument and use 'this' instead.

// === The overload which protects a single column conditionally: ===

/***********************************************************************************************************************************************
Dscr:       Protects DW column conditionally, using a logical expression - protect if true, unprotect if false.
            To protect unconditionally, use the overload without as_protect_expr.
            To remove protection, pass "1=0" as the expression.
************************************************************************************************************************************************
Arg:        adw, as_col_name, as_protect_expr
************************************************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
***********************************************************************************************************************************************/
string   ls_err = ''
string   ls_modify_expr

constant string BG_COLOR__PROTECTED = "536870912" // 536870912 = Transparent; 12632256 = Silver; 67108864 = Button Face
constant string BG_COLOR__EDITABLE = "1073741824" // 1073741824 = Windows Background; 16777215 = White

if IsNull(as_col_name) then
   ls_err = "Column name is NULL."
elseif IsNull(as_protect_expr) then
   ls_err = "Protect expression is NULL."
end if

// Set the expression for the Protect property:
if ls_err = '' then
   ls_modify_expr = as_col_name + ".Protect = ~"0~~t if("+ as_protect_expr + ", 1, 0)~""
   ls_err = adw.Modify(ls_modify_expr)
   if ls_err <> '' then
      if Trim(adw.DataObject) = '' or IsNull(adw.DataObject) then
         ls_err = "DataObject is empty."
      elseif adw.Describe(as_col_name + '.Name') <> as_col_name then
         ls_err = "Column " + as_col_name + " doesn't exist in " + adw.DataObject + "."
      else
         ls_err = "Failed to set Protect property of column " + as_col_name + " (in DataObject" + adw.DataObject + &
                                                   ") to the following expression:~r~n~r~n" + as_protect_expr
      end if
   end if
end if

// Change the background color to make the column looking not-editable:
if ls_err = '' then
   ls_modify_expr = as_col_name + ".Background.Color = ~"0~~t if(" + as_protect_expr + "," + BG_COLOR__PROTECTED + "," + BG_COLOR__EDITABLE + ")~""
   adw.Modify(ls_modify_expr)
end if

if ls_err <> '' then
   MessageBox(this.ClassName() + ".uf_protect_col()", ls_err)
end if

return

// As you see, if an error occurs, uf_protect_col() displays an error message using MessageBox().
// While this isn’t the ideal approach, many developers prefer it because their applications don’t use exception handling.
// However, if you want to follow the correct and recommended practice then add the exception mechanism described https://github.com/Ursego/DWSpy.
// Then, use the next, exception-based version of the function instead (it's the same overload, just using exceptions rather than MessageBox).
// Don't forget to fill the "Throws:" field in the header with n_ex - in all the overloads.

/***********************************************************************************************************************************************
Dscr:       Protects DW column conditionally, using a logical expression - protect if true, unprotect if false.
            To protect unconditionally, use the overload without as_protect_expr.
            To remove protection, pass "1=0" as the expression.
************************************************************************************************************************************************
Arg:        adw, as_col_name, as_protect_expr
************************************************************************************************************************************************
Trows:      n_ex
************************************************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
***********************************************************************************************************************************************/
string   ls_err
string   ls_modify_expr

constant string BG_COLOR__PROTECTED = "536870912" // 536870912 = Transparent; 12632256 = Silver; 67108864 = Button Face
constant string BG_COLOR__EDITABLE = "1073741824" // 1073741824 = Windows Background; 16777215 = White

if IsNull(as_col_name) then
   f_throw(PopulateError(1, "Column name is NULL."))
elseif IsNull(as_protect_expr) then
   f_throw(PopulateError(2, "Protect expression is NULL."))
end if

// Set the expression for the Protect property:
ls_modify_expr = as_col_name + ".Protect = ~"0~~t if("+ as_protect_expr + ", 1, 0)~""
ls_err = adw.Modify(ls_modify_expr)
if ls_err <> "" then
   if Trim(adw.DataObject) = '' or IsNull(adw.DataObject) then
      f_throw(PopulateError(3, "DataObject is empty."))
   elseif adw.Describe(as_col_name + '.Name') <> as_col_name then
      f_throw(PopulateError(4, "Column " + as_col_name + " doesn't exist in " + adw.DataObject + "."))
   else
      f_throw(PopulateError(5, "Failed to set Protect property of column " + as_col_name + " (in DataObject" + adw.DataObject + &
                                                   ") to the following expression:~r~n~r~n" + as_protect_expr))
   end if
end if

// Change the background color to make the column looking not-editable:
ls_modify_expr = as_col_name + ".Background.Color = ~"0~~t if(" + as_protect_expr + "," + BG_COLOR__PROTECTED + "," + BG_COLOR__EDITABLE + ")~""
adw.Modify(ls_modify_expr)

return

// The next overloads are same for the MessageBox and the exception versions.
// Again, for the exception versions, don't forget to fill the "Throws:" field in the header with n_ex (and reflect that in the header comments).

// === The overload which protects a single column unconditionally: ===

/***********************************************************************************************************************************************
Dscr:       Protects DW column unconditionally.
************************************************************************************************************************************************
Arg:        adw, as_col_name
************************************************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
***********************************************************************************************************************************************/

this.uf_protect_col(adw, as_col_name, "1=1")

return

// === The overload which protects a few columns conditionally: ===

/***********************************************************************************************************************************************
Dscr:       Protects DW columns, passed as an array, conditionally, using a logical expression - protect if true, unprotect if false.
            To protect unconditionally, use the overload without as_protect_expr.
            To remove protection, pass "1=0" as the expression.
************************************************************************************************************************************************
Arg:        adw, as_col_names[], as_protect_expr
************************************************************************************************************************************************
Developer:  Michael Zuskin > http://linkedin.com/in/zuskin | http://code.intfast.ca/
***********************************************************************************************************************************************/
int   i
int   li_upper_bound

li_upper_bound = UpperBound(as_col_names[])

for i = 1 to li_upper_bound
   this.uf_protect_col(adw, as_col_names[i], as_protect_expr)
next

return

// === The overload which protects a few columns unconditionally: ===

/***********************************************************************************************************************************************
Dscr:       Protects DW columns, passed as an array, unconditionally.
************************************************************************************************************************************************
Arg:        adw, as_col_names[]
************************************************************************************************************************************************
Developer:  Michael Zuskin > http://linkedin.com/in/zuskin | http://code.intfast.ca/
***********************************************************************************************************************************************/

this.uf_protect_col(adw, as_col_names[], "1=1")

return