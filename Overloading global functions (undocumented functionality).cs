// The official PowerBuilder documentation says:

// "you cannot overload global functions."

// It's true regarding the Function Painter. But there is a secret: we can overload global functions by editing their code manually!
// The steps:
// 1. Create a normal, not-overloaded global function, save and close it.
// 2. Re-open in with "Edit Source".
// 3. Add the needed overloaded versions manually. For that, copy-paste the existing function as many times as neeedd, change the signatures and customize the code. Save.

// When you later reopen it in the usual way, there’s no indication that it’s overloaded — you only see one version.  
// Note that you see the overload which goes last in the source code.  
// So, the explanatory header comment must be added to it.  
// That comment should clearly explain what is going on here, using a fragment like:

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// This function is overloaded. To see all the overloads, open the function in the "Edit Source" mode.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

// I’ve explained how to overload global functions, but only for cases where you need to override existing ones.
// Avoid creating new global functions. Instead, define or modify classes, where function overloading is straightforward and all versions are immediately visible.

// However, this repository describes three global functions I created from scratch, each with overloads:

// * IfNull() - https://github.com/Ursego/PowerBuilder/blob/main/IfNull().cs
// * iif() - https://github.com/Ursego/PowerBuilder/blob/main/iif().cs
// * iin() - https://github.com/Ursego/PowerBuilder/blob/main/iin().cs

// This was done intentionally because these functions are purely technical (not business-related) and don't belong to particular areas of functionality.
// They recreate features that should have been implemented in PowerBuilder using keywords or operators.  
// I use these functions so often in my code that I almost see them as part of the PowerScript language itself.  
// If I had to prefix them with an object or class name every time, it would make my code unnecessarily verbose.