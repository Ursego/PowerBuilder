// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The function iin(<value>, <array>) reports if the value, passed as the 1st argument, appears in the array, passed as the 2nd argument.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Writing Oracle PL/SQL code, I always enjoy the ability to use the SQL's IN clause in a procedural environment:

if i_day in ('SATURDAY', 'SUNDAY') then
   v_is_weekend := TRUE;
end if;

// Of course, we can mimic this in PowerScript using a CHOOSE CASE construction — the only cost is one additional line of code:

choose case as_day
case 'SATURDAY', 'SUNDAY'
   lb_weekend = TRUE
end choose

// Now, see this PL/SQL code - we assign the result of a comparison to a Boolean variable directly:

v_is_weekend BOOLEAN := (i_day IN ('SATURDAY', 'SUNDAY'));

// Unfortunately, we cannot do that in PowerScript since CHOOSE CASE doesn't return a value (as the 'switch' statements does in many language).
// To achieve the same, we need a Boolean expression with OR:

lb_weekend = (as_day = 'SATURDAY' or as_day = 'SUNDAY')

// That solution isn't very elegant — the same variable (ls_day) appears multiple times in the code. And what if you need to compare a variable against 20 values instead of just 2?

// LET'S CREATE WHAT IS MISSING

// The proposed iin() function ("i"nternal "in") checks whether the value passed as the first argument exists in the array provided as the second argument:

lb_weekend = iin(as_day, {'SATURDAY', 'SUNDAY'})

// The fact that we can write array elements directly into the code using curly braces gives us a wide range of applications, and the code is easy to read.

// Of course, you can pass a pre-populated array too. In this case, iin() becomes very similar to the contains() function that arrays have in some languages:

string ls_days[] = {'SATURDAY', 'SUNDAY'}

lb_weekend = iin(as_day, ls_days[])

// It's completely impossible to use an array with CHOOSE CASE — you’re forced to write an entire loop instead of one line of code with iin()!

// Here’s another example, this time with numeric data:

if iin(ll_employee_id, {123, 456, 789}) then

// Overloading of global functions is described here: https://github.com/Ursego/PowerBuilder/blob/main/Overloading%20global%20functions%20(undocumented%20functionality).cs

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/Ursego/DWSpy) then you already have iin() since it's included in the Spy's PBL.
// Otherwise, do the next steps:

// 1. Go to https://github.com/Ursego/DWSpy.
// 2. Right-click spy.pbl and save it among other PBLs of your app.
// 3. Add spy.pbl to the end of your app’s library list. That PBL contains a few objects, but we need only iin.