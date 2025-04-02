// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The function iif(<boolean expression>, <1st value>, <2nd value>) returns the 1st value if the boolean expression is TRUE; otherwise, it returns the 2nd value.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// It works similar to the ternary operator in C-like languages (<cond> ? <if true> : <if false>; ) and the IIf function in Visual Basic (where "IIf" stands for "I"nternal "If"):

ls_sql = iif(Len(as_err_msg) > 0, "ROLLBACK", "COMMIT")

// This approach helps keep scripts shorter by eliminating unnecessary if...else constructions. Each use of iif() saves four extra lines of code.
// Here's how the fragment would look without iif():

if Len(as_err_msg) > 0 then
   ls_sql = "ROLLBACK"
else
   ls_sql = "COMMIT"
end if

// CAUTION!
// If you pass expression as the second or third argument, they are evaluated before being passed to the function — regardless of the result of the Boolean condition.
// So, do NOT use iif() to protect against issues like division by zero or accessing a non-existent row in a DataWindow (as you might with the ternary operator in C-like languages):

ll_result = iif(ll_2 <> 0, ll_1 / ll_2, 0) // runtime error if ll_val_2 = 0!!!
ls_name = iif(ll_row > 0, dw_emp.object.name[ll_row], "No name") // runtime error if ll_row = 0!!!

// The function is overloaded for the following datatypes: string, long, decimal, date, time, datetime, PowerObject.
// Overloading of global functions is described here: https://github.com/Ursego/PowerBuilder/blob/main/Overloading%20global%20functions%20(undocumented%20functionality).cs
// Obviously, the 2nd and the 3rd arguments and the returned value must be of a same datatype.

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/Ursego/DWSpy) then you already have iif() since it's included in the Spy's PBL.
// Otherwise, do the next steps:

// 1. Go to https://github.com/Ursego/DWSpy.
// 2. Right-click spy.pbl and save it among other PBLs of your app.
// 3. Add spy.pbl to the end of your app’s library list. That PBL contains a few objects, but we need only iif.









