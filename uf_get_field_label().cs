// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_get_field_label()
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Read the Dscr in the function's header comment below.

// Before creating uf_get_field_label(), add the next functions:
//      * uf_replace_all() (to the same class where you want to add uf_get_field_label() - https://github.com/Ursego/PowerBuilder/blob/main/uf_replace_all().cs
//      * iin() - https://github.com/Ursego/PowerBuilder/blob/main/iin().cs

/**********************************************************************************************************************
Dscr:       Receives a field name and returns its label (the text of the corresponding <field name>_t object).
            The label is returned ready to be used in messages - without non-alphabetic characters ("&Name:" ==>> "Name").
            If <field name>_t doesn't exist (which is bad practice!), the function generates a user-friendly label
                from the field name ("first_name" ==>> "First Name") to prevent ugly messages.
***********************************************************************************************************************
Arg:        adw - the DataWindow to search in
            as_field_name
***********************************************************************************************************************
Ret:        string
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
string ls_field_label

ls_field_label = adw.Describe(as_field_name + "_t.Text")
if iin(ls_field_label, {"!", "?"}) /* there is NO text object named "<field name>_t" */ then
   // Try to save the situation and return something which can be used as the column's label:
   ls_field_label = as_field_name
   uf_replace_all(ref ls_field_label, "_", " ") // "first_name" ==>> "first name"
   WordCap(ls_field_label) // "first name" ==>> "First Name"
else // text object, named "<field name>_t", exists
   // Remove semicolon which exsist in many labels on free-form DW:
   if Right(ls_field_label, 1) = ":" then
      ls_field_label = Left(ls_field_label, Len(ls_field_label) - 1) // "First Name:" ==>> "First Name"
   end if
   // Remove ampersands which mark hot-key symbols, displayed to user underlined:
   uf_replace_all(ref ls_field_label, "&&", this.ClassName()) // save double-ampersand (which is displayed as one) from removing by next line
   uf_replace_all(ref ls_field_label, "&", "")
   uf_replace_all(ref ls_field_label, this.ClassName(), "&") // restore the saved ampersand (display it as one)
   // Remove other not-alphabetic symbols which can appear in the label:
   uf_replace_all(ref ls_field_label, "~n~r", " ")
   uf_replace_all(ref ls_field_label, "~r~n", " ")
   uf_replace_all(ref ls_field_label, "~n", " ")
   uf_replace_all(ref ls_field_label, "~r", " ")
   uf_replace_all(ref ls_field_label, "~"", "")
end if

return ls_field_label