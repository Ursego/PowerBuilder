// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The function iin(<value>, <array>) reports if the value, passed as the 1st argument, appears in the array, passed as the 2nd argument.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Writing Oracle PL/SQL code, I always enjoy the ability to use the SQL's IN clause in a procedural environment:

if i_day in ('SATURDAY', 'SUNDAY') then
   v_is_weekend := TRUE;
end if;

// Of course, we can mimic this in PowerScript using a CHOOSE CASE — the only cost is one additional line of code:

choose case as_day
case 'SATURDAY', 'SUNDAY'
   lb_weekend = TRUE
end choose

// Now, see this PL/SQL code - we assign the result of a comparison to a Boolean variable directly:

v_is_weekend BOOLEAN := (i_day IN ('SATURDAY', 'SUNDAY'));

// Unfortunately, we cannot do that in PowerScript since CHOOSE CASE doesn't return a value (as the 'switch' statements does in some languages like Kotlin).
// To achieve the same, we need a Boolean expression with OR:

lb_weekend = (as_day = 'SATURDAY' or as_day = 'SUNDAY')

// That solution isn't very elegant — the same variable (ls_day) appears multiple times in the code. And what if you need to compare a variable against 20 values, not just 2?

// LET'S CREATE THE MISSING

// The proposed iin() function ("i"nternal "in") checks whether the value passed as the first argument exists in the array provided as the second argument:

lb_weekend = iin(as_day, {'SATURDAY', 'SUNDAY'})

// The fact that we can write array elements directly into the code using curly braces gives us a wide range of applications, and the code is easy to read.

// Of course, you can pass a pre-populated array too. In this case, iin() becomes very similar to the contains() function that arrays have in some languages:

string ls_days[] = {'SATURDAY', 'SUNDAY'}

lb_weekend = iin(as_day, ls_days[])

// It's completely impossible to use an array with CHOOSE CASE — you’re forced to write an entire loop instead of one line of code with iin()!

// Here’s another example, this time with numeric data:

if iin(ll_employee_id, {123, 456, 789}) then

// Overloading of global functions is described here: https://github.com/UrsegoPowerBuilder/blob/main/Overloading%20global%20functions%20(undocumented%20functionality).cs

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/UrsegoDWSpy) then you already have iin() since it's included in the Spy's PBL.
// Otherwise, save the next code as iif.srf file, and import it to your app:

HA$PBExportHeader$iin.srf
global type iin from function_object
end type

forward prototypes
global function boolean iin (string as_val, string as_arr[])
global function boolean iin (powerobject ao_val, powerobject ao_arr[])
global function boolean iin (long al_val, long al_arr[])
end prototypes

global function boolean iin (string as_val, string as_arr[]);
int	i
int	li_upper_bound

if IsNull(as_val) then return false

li_upper_bound = UpperBound(as_arr[])
for i = 1 to li_upper_bound
	if as_arr[i] = as_val then
		return true
	end if
next

return false
end function

global function boolean iin (ref powerobject apo_val, ref powerobject apo_arr[]);
int	i
int	li_upper_bound

if IsNull(apo_val) then return false

li_upper_bound = UpperBound(apo_arr[])
for i = 1 to li_upper_bound
	if apo_arr[i] = apo_val then
		return true
	end if
next

return false
end function

global function boolean iin (long al_val, long al_arr[]);/**********************************************************************************************************************
Dscr:	Reports if a value, passed as the 1st arg, appears in the array, passed as the 2nd arg.
      Details: https://github.com/UrsegoPowerBuilder/blob/main/iin().cs

      !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
      This function is overloaded for string, long and PowerObject.
      To see all the overloads, open the function in the "Edit Source" mode.
      !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		
		Examples of use:
		 
		if iin(ll_cust_id, ll_best_customers_arr[]) then...
		if iin(ls_city, ls_best_cities_arr[]) then...
		
		The values list can be inserted inline using brackets - {..., ...}:
		
		if iin(ll_cust_id, {123, 456, 789}) then...
		if iin(ls_city, {"Toronto", "Ottawa"}) then...
***********************************************************************************************************************
Arg:	the value to check (can be string, long or PowerObject)
		the array to search in (must be of the same type as the 1st arg)
***********************************************************************************************************************
Ret:	boolean
***********************************************************************************************************************
Dev:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego
**********************************************************************************************************************/
int	i
int	li_upper_bound

if IsNull(al_val) then return false

li_upper_bound = UpperBound(al_arr[])
for i = 1 to li_upper_bound
	if al_arr[i] = al_val then
		return true
	end if
next

return false

end function
