// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// If the functionality is longer than a few lines then don't implement it in system (built-in) events (such as ButtonClicked, ItemChanged etc.).
// Instead, extract the logic into brand new well-named functions and call them from the events.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// If a system event's script grows from just a few lines and starts to include its own logic, it effectively becomes "implementing functionality".
// At that point, the logic should be extracted into a dedicated function (or dedicated functions).

// You should divide your program into short methods, each performing a single, well-defined task (“separation of concerns”).  
// So, if a built-in event needs to handle different logical tasks, implementing each one in its own function as per best coding practices.

// In fact, some built-in events are really just traffic hubs for routing program flow to the appropriate functions using a choose case structure.

// Here is an example for ButtonClicked (AcceptText() is omitted for simplicity):

choose case dwo.name
case "cb_calc_bonus"
    this.uf_calc_bonus(row)
case "cb_print_summary"
    this.uf_print_summary(row)
end choose

// This approach makes it easier to focus on what happens when the user clicks the button, without being distracted by unrelated processing in the same script.

// Also, consider this: one day, the event's functionality might need to be called from somewhere else.  
// You should avoid calling built-in GUI events directly — unless you're using 'call super' from a descendant.  
// On the other hand, calling a well-named function is simple and elegant.  
// It also gives developers the flexibility to add arguments in the future to pass in the necessary data — something that’s not possible with built-in events.

// @@@ ItemChanged

// Sometimes, the processing of one particular field in ItemChanged includes a few short actions unrelated to each other.
// Since they are short, creating a function for each such action would be an overkill.
// So, I extract the fragment for that specific field into one function, and name it using the pattern uf_<built-in event>__<column name> (note the two the underscores).
// For example: uf_itemchanged__birth_date, uf_itemchanged__country_id.

// Each such a function must return the same values which ItemChanged returns:
// 0 - Accept the new value and allow the focus to change (the default)
// 1 - Reject the new value and don't allow focus to change; fire ItemError 
// 2 - Restore the original value without firing ItemError and allow the focus to change

// If the ItemChanged event is doing nothing else besides calling such functions, the script is extremely simple:

choose case dwo.name
case "emp_status"
    return this.uf_itemchanged__emp_status(row, data)
case "country_id"
    return this.uf_itemchanged__country_id(row, data)
end choose

return 0

// If there is something else to do, then use this pattern:

int li_rc = 0

choose case dwo.name
case "birth_date"
    li_rc = this.uf_itemchanged__birth_date(row)
case "country_id"
    li_rc = this.uf_itemchanged__country_id(row)
end choose
if li_rc <> 0 then return li_rc

// ...do something else...

return 0

// It’s perfectly reasonable to end up with many uf_itemchanged__XXX functions - it has happened to me many times.


