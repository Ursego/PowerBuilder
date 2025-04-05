// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_lookup_display() returns the display value of a DW field with a DropDown
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// LookUpDisplay() handles this task if the string is in the Primary! buffer:

ls_expr = "Evaluate('LookUpDisplay(order_status)', " + String(ll_row) + ")"
ls_order_status_desc = dw_test.Describe(ls_expr)

// Unfortunately, PowerBuilder has no built-in way to do the same for buffers other than Primary!.
// You might ask: how can you get the displayed value of a field that's not actually displayed?
// Sometimes, we need the value that would be displayed if the record were in the Primary! buffer.
// A classic example is building an error message during pre-save validation, when the failed row is in the Filter! or Delete! buffer.

// Naturally, uf_lookup_display() should be placed in your base DataWindow ancestor:

/**********************************************************************************************************************
Dscr:       Returns the display value of a DW field with a DropDown. Works on any DW buffer.
            To deal with the Primary! buffer only, use the overloaded function with 2 arguments.
***********************************************************************************************************************
Arg:        al_row - long - row number
            a_buf  - DWBuffer - buffer
            as_col - string - name of column with drop-down
***********************************************************************************************************************
Ret:        the lookup-display value (string)
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
datetime          ldt_val
date              ld_val
time              lt_val
int               li_dwc_row
string            ls_expr
string            ls_val
string            ls_serch_expr
string            ls_data_col
string            ls_data_col_type
string            ls_display_col
string            ls_display_col_type
string            ls_display_value
string            ls_actual_format
DataWindowChild   ldwc

constant string FORMAT_D = 'dd/mm/yyyy' // date format
constant string FORMAT_T = 'hh:mm:ss' // time format

if a_buf = Primary! then
   ls_expr = "Evaluate('LookUpDisplay(" + as_col + ")', " + String(al_row) + ")"
   ls_display_value = this.Describe(ls_expr)
   return ls_display_value
end if

// For Filter! & Delete! buffers, we need to work harder since LookUpDisplay doesn't work with them...

this.GetChild(as_col, ref ldwc)
ls_actual_format = this.GetFormat(as_col)

// Get the Data Column and Display Column and their types:
ls_data_col = this.Describe(as_col + '.dddw.datacolumn')
ls_display_col = this.Describe(as_col + '.dddw.displaycolumn')
ls_data_col_type = this.Describe(as_col + '.coltype')
ls_data_col_type = Lower(Left(ls_data_col_type, 5))
ls_display_col_type = ldwc.Describe(ls_display_col + '.coltype')
ls_display_col_type = Lower(Left(ls_display_col_type, 5))

// Prepare the expression to find the row in Child DW:
choose case ls_data_col_type
case 'decim', 'numbe', 'long', 'ulong', 'real', 'int' // numeric
   ls_val = String(this.GetItemNumber(al_row, as_col, a_buf, false))
   ls_serch_expr = ls_data_col + '=' + ls_val
case 'char', 'char(' // string
   ls_val = this.GetItemString(al_row, as_col, a_buf, false)
   ls_serch_expr = ls_data_col + '="' + ls_val + '"'
case 'datet', 'times' // datetime, timestamp
   ldt_val = this.GetItemDateTime(al_row, as_col, a_buf, false)
   ls_val = String(ldt_val, FORMAT_D + " " + FORMAT_T)
   ls_serch_expr = 'String(' + ls_data_col + ', "' + FORMAT_D + " " + FORMAT_T + '")="' + ls_val + '"'
case 'date' // date
   ld_val = this.GetItemDate(al_row, as_col, a_buf, false)
   ls_val = String(ld_val, FORMAT_D)
   ls_serch_expr = 'String(' + ls_data_col + ', "' + FORMAT_D + '")="' + ls_val + '"'
case 'time' // time
   lt_val = this.GetItemTime(al_row, as_col, a_buf, false)
   ls_val = String(lt_val, FORMAT_T)
   ls_serch_expr = 'String(' + ls_data_col + ', "' + FORMAT_T + '")="' + ls_val + '"'
end choose

// Find the row in Child DW:
li_dwc_row = ldwc.Find(ls_serch_expr, 1, ldwc.RowCount())
if IsNull(li_dwc_row) or li_dwc_row < 1 then return ''

// Get value of the Display Column in that row:
choose case ls_display_col_type
case 'decim', 'numbe', 'long', 'ulong', 'real', 'int' // numeric
   ls_display_value = String(ldwc.GetItemNumber(li_dwc_row, ls_display_col))
case 'char', 'char(' // string
   ls_display_value = ldwc.GetItemString(li_dwc_row, ls_display_col)
case 'datet', 'times' // datetime, timestamp
   ls_display_value = String(ldwc.GetItemDateTime(li_dwc_row, ls_display_col), ls_actual_format)
case 'date' // date
   ls_display_value = String(ldwc.GetItemDate(li_dwc_row, ls_display_col), ls_actual_format)
case 'time' // time
   ls_display_value = String(ldwc.GetItemTime(li_dwc_row, ls_display_col), ls_actual_format)
end choose

return ls_display_value

// You can also create an overload for the the Primary! buffer. The arguments: (al_row, as_col).
// That will not shorten your scripts, but uf_lookup_display() looks more elegant than Describe("Evaluate('LookUpDisplay...:
return this.uf_lookup_display(al_row, Primary!, as_col)

// And the final chord - an overload for a Free Form DW, with only one argument - as_col:
return this.uf_lookup_display(1, Primary!, as_col)