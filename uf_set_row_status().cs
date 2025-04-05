// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// To be used instead of SetItemStatus() which doesn't change any old status to any other status
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// You can find the problem combinations in this table: https://i.postimg.cc/KjLHpBb8/Set-Item-Status.jpg
// As can see, some statuses cannot be set directly — you need a second call to SetItemStatus() to achieve the desired result.  
// One example: changing the row status from New! to NotModified! simply doesn’t work. You have to first set it to DataModified!, and only then can you change it to NotModified!.  
// Some transitions are technically allowed but don’t behave as expected. For instance, changing from NewModified! to NotModified! will actually change to New!.  
// To encapsulate all that complexity (so you can forget about it for good), create the uf_set_row_status function.
// Its name makes it clear that it sets the ROW status, not the COLUMN status.  

// You can place this function in a class of general-purpose utilities, or another appropriate class in your application.
// If you're putting it in a DataWindow ancestor, remove the "adw" argument and use "this" instead.

/**********************************************************************************************************************
Dscr:       Changes the item status of the passed row in a DataWindow.
            Call this function instead of SetItemStatus() which doesn't change any old status to any new status.
            Works on any buffer; to work with the Primary! buffer, use the overloaded version.
***********************************************************************************************************************
Arg:        DataWindow     adw
            long           al_row
            DWItemStatus   a_desired_status
            DWBuffer       a_buf
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
DWItemStatus l_existing_status

l_existing_status = adw.GetItemStatus(al_row, 0, a_buf)

choose case true
case l_existing_status = NewModified! and a_desired_status = New!
   adw.SetItemStatus(al_row, 0, a_buf, NotModified!) // that makes it New! as desired
   return // we are done
case l_existing_status = New! and a_desired_status = NotModified!
   adw.SetItemStatus(al_row, 0, a_buf, DataModified!) // temporarily change to an intermediate status
case l_existing_status = NewModified! and a_desired_status = NotModified!
   adw.SetItemStatus(al_row, 0, a_buf, DataModified!) // temporarily change to an intermediate status
case l_existing_status = DataModified! and a_desired_status = New!
   adw.SetItemStatus(al_row, 0, a_buf, NotModified!) // temporarily change to an intermediate status
end choose

adw.SetItemStatus(al_row, 0, a_buf, a_desired_status)

return

// Also, create an overload without a_buf which works with the Primary! buffer:

/**********************************************************************************************************************
Dscr:       Changes the item tatus of the passed row in a DataWindow.
            Call this function instead of SetItemStatus() which doesn't change any old status to any new status.
            Works only on the Primary! buffer; to work with other buffers, use the overloaded version.
***********************************************************************************************************************
Arg:        DataWindow     adw
            long           al_row
            DWItemStatus   a_desired_status
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
this.uf_set_row_status(adw, al_row, a_desired_status, Primary!)

// And the last thing you need to do is create both overloads for the DataStore (just Ctrl+H DataWindow to DataStore, and adw to ads).