// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// To pass errors from functions outwards, throw exceptions rather than return a success/error code (like 1/-1).
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// The tradition of returning success or failure codes (like 1/-1) dates back to the early days, when exceptions didn’t yet exist in PowerBuilder.
// This tradition has significant drawbacks: it makes code more error-prone (just forget to check a return code, and the error goes unnoticed), longer, and messier.
// Of course, we have no choice but to check return codes from existing functions, but let’s take a modern approach when creating new ones.
// Let’s take advantage of modern tools rather than stick to old habits!
// Please read the following quotes.

// * From Wikipedia (https://en.wikipedia.org/wiki/Defensive_programming):
// "Generally speaking, it is preferable to throw intelligible exception messages that enforce part of your API contract and guide the client programmer instead of returning values
// that a client programmer is likely to be unprepared for and hence minimize their complaints and increase robustness and security of your software."

// * From the book "OCA: Oracle Certified Associate Java SE 8 Programmer I Study Guide: Exam 1Z0-808":
// "Try to avoid return codes. Return codes are commonly used in searches, so programmers are expecting them. In other methods, you will take your callers by surprise by
// returning a special value. An exception forces the program to deal with them or end with the exception if left unhandled, whereas a return code could be accidentally ignored
// and cause problems later in the program. An exception is like shouting, "Deal with me!""

// * From the book "Clean Code":
// "It might seem odd to have a section about error handling in a book about clean code. Error handling is just one of those things that we all have to do when we program.
// Input can be abnormal and devices can fail. In short, things can go wrong, and when they do, we as programmers are responsible for making sure that our code does what it needs
// to do. The connection to clean code, however, should be clear. Many code bases are completely dominated by error handling. When I say dominated, I don't mean that error handling
// is all that they do. I mean that it is nearly impossible to see what the code does because of all of the scattered error handling. Error handling is important, but if it obscures
// logic, it's wrong.
// Use Exceptions Rather Than Return Codes
// Back in the distant past there were many languages that didn't have exceptions. In those languages the techniques for handling and reporting errors were limited.
// You either set an error flag or returned an error code that the caller could check. The problem with these approaches is that they clutter the caller.
// The caller must check for errors immediately after the call. Unfortunately, it's easy to forget. For this reason it is better to throw an exception when you encounter an error.
// The calling code is cleaner. Its logic is not obscured by error handling."

// To throw an exception from a PB function, fill the "Throws:" field in the function's header (signature) with class Exception or its descendant.

// When the exceptions mechanism is in use, functions are called in the simplest possible way:

uf_fly(ls_animal)

// There is no terrible mess like

li_rc = uf_fly(ls_animal)
if li_rc = -1 then
    MessageBox("Ohhh!!!", "Cows cannot fly!")
    return -1 // ...which must be checked in the calling script as well, and so on, and so on... until somebody forgets and the error is silently swallen...
end if

// HOW TO DEAL WITH FUNCTIONS THAT THROW EXCEPTIONS

// The rule is simple: if script A calls script B and script B throws an exception, script A has exactly two options, enforced by the compiler:

// 1. Handle the exception within the script. To do this, script A must wrap the call to script B in a try...catch block.
// 2. Propagate the exception outward. In this case, the responsibility for handling it shifts to the caller.

// WHAT'S WRONG WITH BUILT-IN PB EXCEPTIONS?

// Throwing an exception in PB 8 or later is easy, but in my opinion, three important requirements must be considered.

// First, the code that throws the exception must be compact.
// Since it's technical code embedded in business logic, it should take no more than one line, like "throw new Exception" in Java or C#.
// Therefore, the following solution is absolutely NOT acceptable:

Exception l_ex

[...code...]
if [problem 1] then
   l_ex = create Exception
   l_ex.SetMessage("[error message 1] in n_XXX.uf_YYY ")
   throw l_ex
end if

[...code...]
if [problem 2] then
   l_ex = create Exception
   l_ex.SetMessage("[error message 2] in n_XXX.uf_YYY ")
   throw l_ex
end if

// Imagine a situation where the script throws many exceptions. Potentially, hundreds exceptions are thrown throughout the application.
// Each time, an exception object must be created, populated, and thrown. In such a scenario, the actual business logic becomes difficult to see in all the clutter.
// This is precisely why exceptions are rarely used in PowerBuilder - even though everybody agdees with the idea.
// Hopefully, a future version of PB will introduce a concise, one-line exception-throwing statement similar to those in Java or C#.
// In the meantime, I want to provide a very simple f_throw() global function.
// It returns (None), accepts as_msg String argument and thows Exception. The code:
Exception	l_ex
l_ex = create Exception
l_ex.SetMessage(as_msg)
throw l_ex
// You can implement this function as a public method of an NVO if you prefer, but I recommend making it a global function, even though I generally avoid them.
// In this case, it effectively plays the role of the throw keyword found in other languages.

// Previously, our uf_fly() function returned -1:

if as_animal = "Cow" then return -1

// Now it throws an exception:

if as_animal = "Cow" then f_throw("Cows cannot fly!")

// And the usage bacame super-simple, as we wanted:

uf_fly(ls_animal)

// Notice another advantage: error handling is now encapsulated within the function.
// It determines which error message to display without requiring the caller to interpret error codes.
// This is especially useful when a function can fail for different reasons or is called from multiple places:
// 		instead of returning -1, -2, -3, etc., and making each caller map these codes to error messages, the function handles error reporting itself.
// This eliminates code duplication and reduces the risk of bugs.

// STILL NOT PERFECT...

// This simple f_throw() function is already useful — certainly better than nothing.
// However, it lacks two important features available in C# and Java:

// 1. The error message should identify the class and script where the exception was thrown.
// 2. This information should be captured automatically, without requiring you to specify it manually every time you throw an exception.

// A READY SOLUTION WHICH SOLVES ALL THESE PROBLEMS

// In the solution below, all three operations - creating, populating, and throwing the exception - are combined into an enhanced version of f_throw():

f_throw(PopulateError(0, "[error message]"))

// The PopulateError() function, called within the argument list of f_throw(), automatically captures key details about the exception,
//		such as the class name, script name, and even the line number, and stores them in the Error object.

// Then, f_throw() function performs three operations:
// 1. Creates an instance of Exception, more precisely, its descendant n_ex (which I will introduce later).
// 2. Populates that exception object from the Error object.
// 3. Throws the exception.

// The uf_msg() function, called from within the exception handler, uses this data to construct a clear, informative error message.
// Optionally, it can also log the error to a database table or file, or even send an email to the developer.

// These additional actions must be implemented separately.
// The n_ex object provides a placeholder for custom logic through the uf_write_to_log() function, which does nothing by default.

// Here's what it looks like in practice:

[...code...]
if [problem 1] then f_throw(PopulateError(1, "[error message 1]"))

[...code...]
if [problem 2] then f_throw(PopulateError(2, "[error message 2]"))

// The numeric code passed as the first argument to PopulateError() helps identify the specific point of failure when multiple exceptions can be thrown within the same script.
// Pass 0 if you don't need this information.

// EXAMPLE OF USE:

// Suppose the following code appears on line 17 of the function wf_data_is_ok_before_closing_window() in the window w_emp:

f_throw(PopulateError(3, "Something terrible happened!"))

// The message, displayed by uf_msg(), will have the title "EXCEPTION 3 TROWN" and the following wording:

Window: w_emp
Script: wf_data_is_ok_before_closing_window
Line: 17

Something terrible happened!

// If you pass 0 instead of the 3, the title will be "EXCEPTION TROWN".

// HOW TO ADD THE SOLUTION TO THE APPLICATION?

// If you have added the DataWindow Spy (https://github.com/Ursego/DWSpy) then you already have n_ex and f_throw since they are included in the Spy's PBL.
// Otherwise, do the next steps:

// 1. Go to https://github.com/Ursego/DWSpy/blob/main/spy.pbl.
// 2. Right-click "View raw" and save spy.pbl among other PBLs of your app.
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
if IfNull(ls_xxx, '') = '' then f_throw(PopulateError(0, "ls_xxx is empty.")) // IfNull(): https://github.com/Ursego/PowerBuilder/blob/main/IfNull().cs
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
   // IfNull(<checked value>, <alternative value>) returns the first arg if it is NOT NULL; otherwise, it returns the second arg. Comes with spy.pbl.
   f_throw(PopulateError(0, "Argument as_xxx contains illegal value " + IfNull("'" + as_xxx + "'", "NULL") + ". It must be 'aaa', 'bbb' or 'ccc'."))
end choose

// A try...catch block (for business exceptions which don't have to propagate outword):

try
   // ...your logic...
catch(n_ex e)
   e.uf_msg()
end try

// THERE ARE TWO KINDS OF EXCEPTIONS - TECHNICAL AND BUSINESS EXCEPTIONS

// @@@ Technical exceptions:

// They are bugs which should never occur and need to be investigated and fixed.

// Examples include:  
// @ A required parameter is empty.
// @ A CHOOSE CASE block has no branch for a newly added customer status.
// @ A DataStore retrieves no rows when at least one is expected.
// @ A DataStore retrieves more than one record when maximum one is expected.

// Throw technical exceptions, but don't catch them - let SystemError do that.

// @@@ Business exceptions:

// Not bugs. They absolutely can happen during the normal execution of the application. In fact, they are the means to branch execution flow.
// A typical example is an Oracle PL/SQL user-defined EXCEPTION.

// Examples of situations:
// @ User tries to archive an unpaid order.
// @ User clicks OK button when no row in a DW is selected.
// @ User closes a window with unsaved changes.

// While the purpose of technical exceptionsis to inform about bugs, the purpose of business exceptions is to handle special business situations.

// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
// f_throw should not be used for business exceptions.
// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

// The goal of f_throw() is calling your function without "li_rc = ...". However, with business exceptions, you DO have to process the returned info
// (be it a return value, a ref argument or an exception), so f_throw() provides no advantage, just adds mess.