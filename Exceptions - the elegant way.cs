// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// To pass errors from functions outwards, throw exceptions rather than return a success/error code (like 1/-1).
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// From Wikipedia (https://en.wikipedia.org/wiki/Defensive_programming):
// "Prefer exceptions to return codes
// Generally speaking, it is preferable to throw intelligible exception messages that enforce part of your API contract and guide the client programmer instead of returning values that a client programmer is likely to be unprepared for and hence minimize their complaints and increase robustness and security of your software."

// From the book "OCA: Oracle Certified Associate Java SE 8 Programmer I Study Guide: Exam 1Z0-808":
// "...try to avoid return codes. Return codes are commonly used in searches, so programmers are expecting them. In other methods, you will take your callers by surprise by returning a special value. An exception forces the program to deal with them or end with the exception if left unhandled, whereas a return code could be accidentally ignored and cause problems later in the program. An exception is like shouting, "Deal with me!""

// From the book "Clean Code":
// "It might seem odd to have a section about error handling in a book about clean code. Error handling is just one of those things that we all have to do when we program. Input can be abnormal and devices can fail. In short, things can go wrong, and when they do, we as programmers are responsible for making sure that our code does what it needs to do. The connection to clean code, however, should be clear. Many code bases are completely dominated by error handling. When I say dominated, I don't mean that error handling is all that they do. I mean that it is nearly impossible to see what the code does because of all of the scattered error handling. Error handling is important, but if it obscures logic, it's wrong.
// Use Exceptions Rather Than Return Codes
// Back in the distant past there were many languages that didn't have exceptions. In those languages the techniques for handling and reporting errors were limited. You either set an error flag or returned an error code that the caller could check. The problem with these approaches is that they clutter the caller. The caller must check for errors immediately after the call. Unfortunately, it's easy to forget. For this reason it is better to throw an exception when you encounter an error. The calling code is cleaner. Its logic is not obscured by error handling."

// To make a PB function throw an exception, fill the "Throws:" field in the function's header (signature) with class Exception or its descendant.

// When exceptions mechanism is in use, functions are called in the simplest possible way:

uf_do_something()

// As you see, there is no terrible code impurities like

li_rc = uf_do_something()
if li_rc = -1 then
    // ...display an error message...
    return -1
end if

// The tradition of returning success or failure codes (1/-1) dates back to the early days, when exceptions didn’t yet exist in PowerBuilder. But there’s no need to use horses in the age of automobiles! While we still check return codes from existing functions, you should take a modern approach when writing new code.

// HOW TO DEAL WITH FUNCTIONS WHICH THROW EXCEPTIONS

// The rule is simple: if script A calls script B and script B throws an exception, then script A has exactly two options, enforced by the compiler:  

// 1. Handle the exception. To do this, script A must wrap the call to script B in a try...catch block.  
// 2. Propagate the exception outward. In that case, the responsibility of handling the exception shifts to the script that calls script A.

// WHAT'S WRONG WITH BUILT-IN PB EXCEPTIONS?

// Throwing an exception in PB 8 or later is easy, but in my opinion, there are three important requirements that must be considered:

// 1. The error message describing the problem should include the class and script where the issue occurred.
// 2. This information should be populated automatically, not manually typed by the developer each time.
// 3. The code that throws the exception must be compact. Since it's technical code embedded within business logic, it should take no more than one line — similar to throw new Exception in Java or C#. Therefore, the following solution is absolutely NOT acceptable:

Exception l_ex

[...code...]
if [problem 1] then
   l_ex = create Exception
   l_ex.SetMessage("[error message 1] in function uf_XXX of class n_YYY")
   throw l_ex
end if

[...code...]
if [problem 2] then
   l_ex = create Exception
   l_ex.SetMessage("[error message 2] in function uf_XXX of class n_YYY")
   throw l_ex
end if

// Imagine a situation where the script throws many exceptions. Potentially, hundreds exceptions are thrown throughout the application. Each time, an exception object must be created, populated, and thrown. In such a scenario, the actual business logic becomes difficult to see amid all the clutter. This is precisely why exceptions are rarely used in PowerBuilder. Hopefully, a future version of PB will introduce a concise, one - line exception-throwing statement similar to those in Java or C#. In the meantime, I want to provide...

// A READY SOLUTION WHICH SOLVES ALL THESE PROBLEMS

// In the suggested solution, all three actions related to the exception—creation, population, and throwing—are combined into a single function named f_throw(). You can implement it as a public method of an NVO if you prefer, but I recommend using it as a global function, even though I generally avoid them. In this case, it effectively serves the role of the throw keyword found in other languages:

f_throw(PopulateError(0, "[error message]"))

// As you can see, all the described conditions are met. The PopulateError() function  —called within the argument list of f_throw() — automatically captures key details about the exception, such as the class name, script name, and even the line number, and stores them in the Error object.

// The f_throw() function does three things:
// 1. Creates an instance of Exception — more precisely, of its descendant n_ex (provided with the solution).
// 2. Populates it with the data stored in the Error object.
// 3. Throws the new exception.

// The uf_msg() function (which should be called from within the exception handler) knows how to use this data to construct a clear and informative error message. Optionally, it can also write the error to a log table or file, or even send an email to the developer. These additional actions must be implemented individually — n_ex simply provides a placeholder for custom logic via the uf_write_to_log() function, which does nothing by default.

// Here's what it all looks like in practice:

[...code...]
if [problem 1] then f_throw(PopulateError(1, "[error message 1]"))

[...code...]
if [problem 2] then f_throw(PopulateError(2, "[error message 2]"))

// See how much shorter the code is? Now, nothing is stopping you from using exceptions throughout the entire application — you no longer have an excuse to keep returning -1!

// The numeric code passed as the first argument to PopulateError() helps identify the specific problem spot when multiple exceptions may be thrown within the same script; simply pass 0 if you don't need that.

// EXAMPLE OF USE:

// Let’s say the following code appears on line 17 of the function wf_massage_data() in the window w_emp:

f_throw(PopulateError(3, "Something terrible happened!"))

// The message, displayed by uf_msg(), will have the title "EXCEPTION 3 TROWN" and the following wording:

Window: w_emp
Script: wf_massage_data
Line: 17

Something terrible happened!

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/Ursego/DWSpy) then you already have the new exceptions functionality since the related objects are included in the Spy's PBL. Otherwise, do the next steps:

// 1. Go to https://github.com/Ursego/DWSpy.
// 2. Right-click spy.pbl and save it among other PBLs of your app.
// 3. Add spy.pbl to the end of your app’s library list. That PBL contains a few objects, but we need only n_ex and f_throw.
// 4. Add to the Application object's SystemError script:

n_ex lnv_ex

IF IsValid(ErrorObject) AND (ErrorObject.TypeOf() = type n_ex OR ErrorObject.TypeOf().IsDescendantOf(type n_ex)) THEN
    lnv_ex = ErrorObject
    lnv_ex.uf_msg()
    HALT CLOSE
    RETURN 1 // error handled
END IF

// ...if your SystemError has some code, keep it here...

RETURN 0 // error not handled - keep the standard processing of SystemError


// PRE-COOKED CODE FRAGMENTS

// I have collected a few ready code fragments useful in some standard "exceptional" situations. You can save them in a file and and use when needed with some customization:

f_throw(PopulateError(0, ""))
if IsNull(ls_xxx) then f_throw(PopulateError(0, "ls_xxx is NULL."))
if IfNull(ls_xxx, '') = '' then f_throw(PopulateError(0, "ls_xxx is empty.")) // IfNull(): http://code.intfast.ca/viewtopic.php?t=5
if li_rc <> 1 then f_throw(PopulateError(0, "uf_xxx failed. Arguments: ..., ..."))
if not IsValid(ads_xxx) then f_throw(PopulateError(0, "ads_xxx is invalid."))
if ads_XXX.RowCount() < 1 then f_throw(PopulateError(0, "ads_XXX has no rows."))
if ads_XXX.RowCount() <> 1 then f_throw(PopulateError(0, "ads_XXX.RowCount() must be 1, not " + String(ids_XXX.RowCount()) + "."))
if ll_row_count <> 1 then f_throw(PopulateError(0, "Row count must be 1, not " + String(ll_row_count) + "."))
f_throw(PopulateError(0, "Argument as_mode contains illegal value " + IfNull("'" + as_mode + "'", "NULL") + ".")
f_throw(PopulateError(0, "Argument ai_mode contains illegal value '" + IfNull(String(ai_mode), "NULL") + "'.")
f_throw(PopulateError(0, "This function MUST never be called. It MUST be implemented in the descendant class " + this.ClassName() + "."))

// Some defensive programming:

choose case as_xxx
case "aaa"
   // do something
case "bbb"
   // do something else
case else
   f_throw(PopulateError(0, "Argument as_xxx contains illegal value " + IfNull("'" + as_xxx + "'", "NULL") + ". It must be 'aaa', 'bbb' or 'ccc'."))
end choose

// A try...catch block (for business exceptions which not be to propagated outword):

try
   
catch(n_ex e)
   e.uf_msg()
end try

// THERE ARE TWO KINDS OF EXCEPTIONS - TECHNICAL AND BUSINESS EXCEPTIONS

// @@@ Technical exceptions:

// They indicate bugs which should never occur and need to be fixed.

// Examples include:  
// @ A required parameter is passed as empty to a function.  
// @ A CHOOSE CASE block has no branch for a newly added customer status.  
// @ A DataStore retrieves no rows when at least one is expected (likely an issue in the WHERE clause).

// Don't catch technical exceptions - let SystemError do that.

// @@@ Business exceptions:

// They are not bugs and absolutely can happen during the normal execution of the application. In fact, they are the means to branch execution flow.

// Examples of situations:
// @ User tries to archive an order which has not been paid.
// @ User clicks OK button when no row in a DW is selected.
// @ User closes a window with unsaved changes.

// While the purpose of technical exceptionsis to inform the user about a bug, the purpose of business exceptions is to inform calling scripts about special business situations.

// In general, f_throw is not supposed to be used for business exceptions. But you decide to use it, keep in mind that the try...catch block must distinguish it from a technical exception, and re-trow any technical exception as is to be handled in SystemError. For that, a convention must be used. For example, the massages of business exceptions should start with "###".

// First, declare a constant in n_ex, for example:

public constant string NO_CUST_SELECTED = "### NO CUST SELECTED"

// Let's say, you have uf_process_customers() function. Use the contant this way:

ll_row = dw_cust.GetSelectedRow(0)
if ll_row = 0 then
   f_throw(PopulateError(0, n_ex.NO_CUST_SELECTED))
end if

// The fragment of the calling script:

try
   uf_process_customers()
catch(n_ex e)
   if e.GetMessage() = n_ex.NO_CUST_SELECTED then
      MessageBox("No customers selected", "Please select a customer.")
   else
      f_throw(PopulateError(0, e.GetMessage())) // it's a technical exception - re-throw it!!!
   end if
end try

// Using f_throw is very dangerous because a caught technical exception can be forgotten to be re-thrown. That will cut the chain of exception propagation and produce a hidden bug.




