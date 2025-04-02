// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// The function iin(<value>, <array>) reports if the value, passed as the 1st argument, appears in the array, passed as the 2nd argument.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Writing Oracle PL/SQL code, I always enjoy the ability to use the SQL's IN clause in a procedural environment:

if v_day in ('SATURDAY', 'SUNDAY') then
   -- do something
end if;

// Of course, we can mimic this behavior in PowerScript using a CHOOSE CASE construction — the only cost is one additional line of code:

choose case ls_day
case 'SATURDAY', 'SUNDAY'
   // do something
end choose

// Unfortunately, it's not possible to use CHOOSE CASE to assign the result of a comparison to a Boolean variable. To achieve this, we need to use a Boolean expression with OR:

lb_weekend = (ls_day = 'SATURDAY' or ls_day = 'SUNDAY')

// That solution isn't very elegant — the same variable (ls_day) appears multiple times in the code. And what if you need to compare a variable against 30 values instead of just 2?

// Let me introduce the iin() function ("i"nternal "in"). It checks whether the value passed as the first argument exists in the array provided as the second argument:

lb_weekend = iin(ls_day, {'SATURDAY', 'SUNDAY'})

// Of course, you can use a pre-populated array:

string ls_days[] = {'SATURDAY', 'SUNDAY'}

lb_weekend = iin(ls_day, ls_days[])

// It's completely impossible to use an array with CHOOSE CASE—you’re forced to write an entire loop instead of a single line of code with iin()!

// Here’s another example, this time with numeric data:

if iin(ll_employee_id, {123, 456, 789}) then

// Overloading of global functions is described here:
// https://github.com/Ursego/PowerBuilder/blob/main/Overloading%20global%20functions%20(undocumented%20functionality).cs

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/Ursego/DWSpy) then you already have iin() since it's included in the Spy's PBL.
// Otherwise, do the next steps:

// 1. Go to https://github.com/Ursego/DWSpy.
// 2. Right-click spy.pbl and save it among other PBLs of your app.
// 3. Add spy.pbl to the end of your app’s library list. That PBL contains a few objects, but we need only iin.
