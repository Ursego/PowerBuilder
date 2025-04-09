// Data can be copied from one DW/DS to another with ease:

<target DW or DS>.object.data = <source DW or DS>.object.data

// However, there is a potential issue with this approach: if the source contains no rows, a runtime error occurs when attempting to access <source DW or DS>.object.data.
// This problem is resolved with a small, auto-instantiated NVO, n_copy_data.
// The NVO includes four overloaded functions, uf_from_to(<source DW or DS>, <target DW or DS>), that ensure there is at least one row to copy.

// If you wish to integrate these functions into another NVO (or into your ancestor DW/DS), rename them to uf_copy_data_from_to,
//      as the name uf_from_to is meaningful only within the context of n_copy_data, such as in lnv_copy_data.uf_from_to().
// If you add them to your ancestor DW/DS, remove the first argument and use "this" instead.

// Otherwise, save the next code as n_copy_data.sru file, and import it to your app:

$PBExportHeader$n_copy_data.sru
forward
global type n_copy_data from nonvisualobject
end type
end forward

global type n_copy_data from nonvisualobject autoinstantiate
end type

forward prototypes
public function long uf_from_to (ref datawindow adw_from, ref datawindow adw_to)
public function long uf_from_to (ref datastore ads_from, ref datastore ads_to)
public function long uf_from_to (ref datastore ads_from, ref datawindow adw_to)
public function long uf_from_to (ref datawindow adw_from, ref datastore ads_to)
end prototypes

public function long uf_from_to (ref datawindow adw_from, ref datawindow adw_to);/**********************************************************************************************************************
Dscr:	Gets two DWs and copies data from the first to the second one.
***********************************************************************************************************************
Arg:	adw_from - the source DW
		adw_to - the target DW
***********************************************************************************************************************
Ret:	long (number of rows copied)
***********************************************************************************************************************
Dev:	Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long	ll_rows

adw_to.Reset()

ll_rows = adw_from.RowCount() + adw_from.FilteredCount() + adw_from.DeletedCount()
if ll_rows > 0 then
	adw_to.object.data = adw_from.object.data
end if

return ll_rows
end function

public function long uf_from_to (ref datastore ads_from, ref datastore ads_to);/**********************************************************************************************************************
Dscr:	Gets two DSs and copies data from the first to the second one.
***********************************************************************************************************************
Arg:	ads_from - the source DS
		ads_to - the target DS
***********************************************************************************************************************
Ret:	long (number of rows copied)
***********************************************************************************************************************
Dev:	Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long	ll_rows

ads_to.Reset()

ll_rows = ads_from.RowCount() + ads_from.FilteredCount() + ads_from.DeletedCount()
if ll_rows > 0 then
	ads_to.object.data = ads_from.object.data
end if

return ll_rows
end function

public function long uf_from_to (ref datastore ads_from, ref datawindow adw_to);/**********************************************************************************************************************
Dscr:	Gets two DS and DW and copies data from the first to the second one.
***********************************************************************************************************************
Arg:	ads_from - the source DS
		adw_to - the target DW
***********************************************************************************************************************
Ret:	long (number of rows copied)
***********************************************************************************************************************
Dev:	Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long	ll_rows

adw_to.Reset()

ll_rows = ads_from.RowCount() + ads_from.FilteredCount() + ads_from.DeletedCount()
if ll_rows > 0 then
	adw_to.object.data = ads_from.object.data
end if

return ll_rows
end function

public function long uf_from_to (ref datawindow adw_from, ref datastore ads_to);/**********************************************************************************************************************
Dscr:	Gets DW and DS and copies data from the first to the second one.
***********************************************************************************************************************
Arg:	adw_from - the source DW
		ads_to - the target DS
***********************************************************************************************************************
Ret:	long (number of rows copied)
***********************************************************************************************************************
Dev:	Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long	ll_rows

ads_to.Reset()

ll_rows = adw_from.RowCount() + adw_from.FilteredCount() + adw_from.DeletedCount()
if ll_rows > 0 then
	ads_to.object.data = adw_from.object.data
end if

return ll_rows
end function

on n_copy_data.create
call super::create
TriggerEvent( this, "constructor" )
end on

on n_copy_data.destroy
TriggerEvent( this, "destructor" )
call super::destroy
end on

