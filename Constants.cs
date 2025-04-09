// You might think: constants are an obvious and easy topic. Why dedicate an entire article to them?
// The reason is that many even experienced developers do not attach importance to working with constants correctly, but this could make the work easier and reduce errors.

// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constants used only within (or with) a particular object (not throughout the application)
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Such constants can be local, instance or shared. Build their names from two parts:

// 1. The constants group or family (such as ORDER_STATUS or INV_STATUS)
// 2. The mnemonic description of the code (such as OPEN or CLOSED)

// Separate these two parts with two underscores (__), unlike the single underscore used within each part to separate words:

CONSTANTS_GROUP__CODE_DESCRIPTION

// This naming method offers two key benefits:

// • It helps code readers clearly distinguish between the two parts, even when both parts contain multiple words.
// • It allows multiple constants with the same description to coexist within the same scope — for example:

constant string ORDER_STATUS__OPEN = "OPN"
constant string ORDER_STATUS__CLOSED = "CLS"
constant string ORDER_STATUS__CANCELED = "CNC"

constant string INV_STATUS__OPEN = "OPN"
constant string INV_STATUS__CLOSED = "CLS"
constant string INV_STATUS__CANCELED = "CNC"


// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Constants used throughout the application
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Constants that are used globally across the application must be declared in non-autoinstantiated NVOs — one NVO for each constants group (or "family").
// For example, you might have an NVO named n_color for color constants, n_order_status for order status constants, and so on
// Here's a possible set of constants for n_order_status:

public:

   constant string OPEN = "OPN"
   constant string CLOSED = "CLS"
   constant string CANCELED = "CNC"

// If you open any non-autoinstantiated class in "View Source" mode, you'll notice that PowerBuilder automatically creates a global variable of that class's type.
// The name of that variable is the same as the class itself (violating the naming convention — ha-ha!).
// For example, if you create a class named n_order_status and look at its source, you'll see the following line:

global n_order_status n_order_status

// That allows you to access constants just like you would access static fields in C# or Java:

if ls_order_status = n_order_status.OPEN then...

// So, never declare variables of constant NVO types — just use the one that already exists automatically.

// The created NVO must include constants for ALL codes in the group, even if you currently need only one of them.
// This helps developers see the full picture for that family when they look at the declaration section.  

// If the codes are stored in the database, it's a good idea to add a comment with the SQL SELECT statement used to retrieve them.

// For constants that don't belong to any specific family, create a common NVO — for example, name it n_consts.

// Unfortunately, the method described in this topic doesn't create an actual enumeration like in other languages.
// If you need to pass an order status to a function, the argument type will still be string, not n_order_status.
// So it's your responsibility to validate the argument and throw an exception if its value doesn't exist among the constants defined in n_order_status.

// REMARK: You can create real enums for PB .NET targets: https://infocenter.sybase.com/help/index.jsp?topic=/com.sybase.infocenter.dc01261.1252/doc/html/hfr1224776314675.html


// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Use constants always across the whole application
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// If a constant is defined in the application for a value you're using, ALWAYS use the constant instead of a hard-coded value.

// Yes, sometimes it feels easier to just hard-code a value. But what happens if those system codes change in the future?
// This is exactly why we use constants — you only need to update the value in one place, and the change will automatically propagate across the entire application,
//         keeping the logic intact everywhere.

// I'm sure you already use constants in PowerScript comparisons. But if you use them ONLY there, it's not really “across the whole application.”
// There are other situations where application codes are used — and some of those are described below.

// @@@ Use constants in expressions

// Use string concatenation to apply constants in all kinds of expressions — for example, in expressions for functions like Find(), SetFilter(), and Modify(),
//     as well as in expressions for the Required and Protected properties of columns.
// For the Protected property, use the uf_protect_col() function: https://github.com/Ursego/PowerBuilder/blob/main/uf_protect_col().cs

// @@@ Populate Code Tables from constants

// When creating a control with a Code Table (such as a DropDownListBox or RadioButton), populate its Code Table programmatically if constants are defined in the application
//     for the corresponding values — instead of hard-coding them in the DataWindow Painter. This can be done using the SetValue() method of the DataWindow.

// For example, if you need to create a radio button for the field payment_method in your DataWindow, and the constants n_payment_method.CREDIT_CARD (with value "C") and n_payment_method.PAYPAL (with value "P") already exist in your system, use those constants directly instead of hard-coded values:

dw_payment.SetValue("payment_method", 1, "Credit Card~t'" + n_payment_method.CREDIT_CARD + "'")
dw_payment.SetValue("payment_method", 2, "PayPal~t'" + n_payment_method.PAYPAL + "'")

// @@@ Use constants in SQL SELECTs of DWs

// For that, pass them as retrieval arguments.

// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// NULL constants
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Have a ready-to-use set of null values for different data types, instead of calling SetNull() every time you need a null variable.  
// This approach makes the code cleaner and more consistent, and avoids repeating the same function call throughout your code.

// Sometimes we use variables that are set to null — for example, to pass a null to a function, or, conversely, to return a null from a function.
// Developers usually declare a variable and nullify it using SetNull() in the same script where it's used.
// But these two lines (and really, hundreds of lines across the project) can be avoided by declaring null constants for different data types in a globally accessible object.

// Technically, PowerBuilder doesn't allow initializing constants with NULL.
// Instead, we will declare a set of public instance variables using the privatewrite access modifier, and set them to NULL in the constructor.
// This way, we'll have physical variables that effectively behave like constants.
// We can create them in a class that provides general-purpose technical services, such as n_util.

// Step 1: Declare the public privatewrite variables in the instance variable declaration section of the chosen class:

public:

   privatewrite boolean     NULL__BOOLEAN
   privatewrite int         NULL__INT
   privatewrite long        NULL__LONG
   privatewrite dec         NULL__DEC
   privatewrite string      NULL__STRING
   privatewrite char        NULL__CHAR
   privatewrite date        NULL__DATE
   privatewrite time        NULL__TIME
   privatewrite datetime    NULL__DATETIME

// Step 2: Nullify them in the NVO's Constructor:

SetNull(NULL__BOOLEAN)
SetNull(NULL__INT)
SetNull(NULL__LONG)
SetNull(NULL__DEC)
SetNull(NULL__STRING)
SetNull(NULL__CHAR)
SetNull(NULL__DATE)
SetNull(NULL__TIME)
SetNull(NULL__DATETIME)

// Now, instead of

string ls_null
SetNull(ls_null)
return ls_null

// you can simply write

return n_util.NULL__STRING

// Alternatively, you can create the NULL constants in a dedicated class, such as n_null.
// In this case, remove NULL_ from their names, leaving only _BOOLEAN, _INT etc. So, the last example will become

return n_null._STRING