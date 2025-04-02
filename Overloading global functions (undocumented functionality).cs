// The official PowerBuilder documentation says:

// "you cannot overload global functions."

// It's true that we cannot overload global functions in the Function Painter.
// But there is a secret: we can do that by editing code manually!
// The steps:
// 1. Create a normal, not-overloaded global function, save and close it.
// 2. Re-open in with "Edit Source".
// 3. Add the needed overloaded versions manually. For that, copy-paste the existing function as many times as neeedd, change the signatures and customize the code. Save.

// When you later reopen it in the usual way, there’s no indication that it’s overloaded — you only see one version.  
// Note that the version you see is the last overload in the source code.  
// So, the explanatory header comment must be added to the last overload.  
// That comment should clearly explain what the hell is going on here, using a fragment like:

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// This function is overloaded. To see all the versions, open it in the "Edit Source" mode.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!