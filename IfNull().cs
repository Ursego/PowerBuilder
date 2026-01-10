// The function IfNull(<checked value>, <alternative value>) returns the first arg if it is NOT NULL; otherwise, it returns the second arg.
// It mimics the nvl() function in Oracle. The name nvl stands for "Null VaLue".  
// In SQL Server and Sybase, a similar function is called IsNull(), which is confusing since it's not a Boolean function.  
// C# uses ?? — the null-coalescing operator.  
// Some languages, like Kotlin, use ?: — the Elvis operator (if you tilt your head to the left, it looks like Elvis’s haircut and eyes).  

// However, many languages don’t have this functionality at all, so developers are forced to check for null manually.  
// PowerScript is one of those languages, but nothing stops us from creating a global function for it.

// The proposed IfNull() function is overloaded for the following datatypes: string, long, double, boolean, PowerObject.
// Overloading of global functions is described here: https://github.com/Ursego/PowerBuilder/blob/main/Overloading%20global%20functions%20(undocumented%20functionality).cs
// Obviously, both the arguments and the returned value must be of a same datatype.

// Examples of use:

ll_row_count = IfNull(uf_get_row_count(), 0)
li_min_allowed_age = IfNull(uf_get_min_allowed_age(), 18)
ll_total_hours = ll_hours_per_day * IfNull(li_num_of_days, 1) // prevent NULL in li_num_of_days from ruining the calculation
ls_full_name = ls_first_name + " " + IfNull(ls_mid_name + " ", "") + ls_last_name // ls_mid_name is optional
ls_err = "Invalid as_mode " + IfNull("'" + as_mode + "'", "NULL") + "." // prevent NULLifying of ls_err
ls_err = "Invalid ai_mode '" + IfNull(String(ai_mode), "NULL") + "'." // cast to String since both the arguments must be of a same datatype
lb_value_changed = (IfNull(ls_new_value, "~") <> IfNull(ls_old_value, "~")) // comparison when two NULLs are considered equal values and "~" is an illegal value
lb_empty = (IfNull(ls_xxx, '') = '') // both NULL and empty string are treated as "no value" 
lcb_clicked_button = IfNull(acb_clicked_button, cb_cancel) // works fine with objects

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/Ursego/DWSpy) then you already have IfNull() since it's included in the Spy's PBL.
// Otherwise, do the next steps:

// 1. Go to https://github.com/Ursego/DWSpy.
// 2. Right-click spy.pbl and save it among other PBLs of your app.
// 3. Add spy.pbl to the end of your app’s library list. That PBL contains a few objects, but we need only IfNull.




