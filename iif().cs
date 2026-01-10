// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The function iif(<boolean expression>, <1st value>, <2nd value>) returns the 1st value if the boolean expression is TRUE; otherwise, it returns the 2nd value.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// It works similar to the ternary operator in C-like languages (<cond> ? <if true> : <if false>; ) and the IIf function in Visual Basic (where "IIf" stands for "I"nternal "If"):

ls_sql = iif(Len(as_err_msg) > 0, "ROLLBACK", "COMMIT")

// This approach helps keep scripts shorter by eliminating unnecessary if...else constructions. Each use of iif() saves four extra lines of code.
// Here's how the fragment would look like without iif():

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
// Overloading of global functions is described here: https://github.com/UrsegoPowerBuilder/blob/main/Overloading%20global%20functions%20(undocumented%20functionality).cs
// Obviously, the 2nd and the 3rd arguments and the returned value must be of a same datatype.

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/UrsegoDWSpy) then you already have iif() since it's included in the Spy's PBL.
// Otherwise, save the next code as iif.srf file, and import it to your app:

HA$PBExportHeader$iif.srf
global type iif from function_object
end type

forward prototypes
global function powerobject iif (boolean ab_condition, powerobject apo_if_true, powerobject apo_if_false)
global function datetime iif (boolean ab_condition, datetime adt_if_true, datetime adt_if_false)
global function date iif (boolean ab_condition, date ad_if_true, date ad_if_false)
global function time iif (boolean ab_condition, time at_if_true, time at_if_false)
global function decimal iif (boolean ab_condition, decimal adc_if_true, decimal adc_if_false)
global function long iif (boolean ab_condition, long al_if_true, long al_if_false)
global function string iif (boolean ab_condition, string as_if_true, string as_if_false)
end prototypes

global function powerobject iif (boolean ab_condition, powerobject apo_if_true, powerobject apo_if_false);
if ab_condition then return apo_if_true
return apo_if_false
end function

global function datetime iif (boolean ab_condition, datetime adt_if_true, datetime adt_if_false);
if ab_condition then return adt_if_true
return adt_if_false
end function

global function date iif (boolean ab_condition, date ad_if_true, date ad_if_false);
if ab_condition then return ad_if_true
return ad_if_false
end function

global function time iif (boolean ab_condition, time at_if_true, time at_if_false);
if ab_condition then return at_if_true
return at_if_false
end function

global function decimal iif (boolean ab_condition, decimal adc_if_true, decimal adc_if_false);
if ab_condition then return adc_if_true
return adc_if_false
end function

global function long iif (boolean ab_condition, long al_if_true, long al_if_false);
if ab_condition then return al_if_true
return al_if_false
end function

global function string iif (boolean ab_condition, string as_if_true, string as_if_false);/**********************************************************************************************************************
Dscr:	Returns the value of the 2nd or the 3rd arg depending on the expression in the 1st arg.
      Details: https://github.com/UrsegoPowerBuilder/blob/main/iif().cs
      !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
      This function is overloaded for string, long, decimal, date, time, datetime and PowerObject.
      To see all the overloads, open the function in the "Edit Source" mode.
      !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
***********************************************************************************************************************
Arg:	boolean - the expression to evaluate;
		string - the value to return if the expression is true;
		string - the value to return if the expression is false
***********************************************************************************************************************
Ret:	string
***********************************************************************************************************************
Dev:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego
**********************************************************************************************************************/
if ab_condition then return as_if_true

return as_if_false

end function
