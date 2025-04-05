// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_ds_from_array() converts an array to a DataStore
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// It takes an array and creates a DataStore with a single column named "the_col" containing the array’s values.
// This is useful when working with arrays, but processing them becomes faster or more convenient by using a DataStore —
// thanks to its efficient built-in functions that eliminate the need for manual looping.  

// A classic example is sorting a large array: it's faster to convert it to a DataStore and use Sort().

// First, add the function uf_get_pb_version() to your utility NVO: https://github.com/Ursego/PowerBuilder/blob/main/uf_get_pb_version().cs
// Then, add the next two overloads of uf_ds_from_array() — one for string arrays and one for long arrays:

// @@@ String:

/**********************************************************************************************************************
Dscr:       Gets a STRING array and creates a DS, the only column of which (named "the_col") contains the array's values:

            After the values, previously converted to a DataStore by this function, have been processed, they can be
            easily converted back to an array by accessing the DataStore's property object.the_col.current:
      
            lds_temp = uf_ds_from_array(ls_arr[])
            ...process the data in lds_temp...
            ls_arr[] = lds_temp.object.the_col.current // <<< convert back to an array
            destroy lds_temp
      
            For a LONG array, use another overload.
***********************************************************************************************************************
Arg:         as_arr[]
***********************************************************************************************************************
Ret:         DataStore
***********************************************************************************************************************
Developer:   Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego
**********************************************************************************************************************/
string      ls_source
string      ls_err
string      ls_ver
DataStore   lds

ls_ver = this.uf_get_pb_version() // https://github.com/Ursego/PowerBuilder/blob/main/uf_get_pb_version().cs
ls_source = 'release ' + ls_ver + '; datawindow() table(column=(type=char(10000) name=the_col dbname="the_col") )'

lds = create DataStore
lds.create(ls_source, ls_err)

if UpperBound(as_arr) > 0 then
   lds.object.the_col.current = as_arr
end if

return lds

// @@@ Long:

/**********************************************************************************************************************
Dscr:       Gets a LONG array and creates a DS, the only column of which (named 'the_col') contains the array's values.
            After the values, previously converted to a DataStore by this function, have been processed, they can be
            easily converted back to an array by accessing the DataStore's property object.the_col.current:
            
            lds_temp = uf_ds_from_array(ll_arr[])
            ...process the data in lds_temp...
            ll_arr[] = lds_temp.object.the_col.current // <<< convert back to an array
            destroy lds_temp
            
            For a STRING array, use another overload.
***********************************************************************************************************************
Arg:         al_arr[]
***********************************************************************************************************************
Ret:         DataStore
***********************************************************************************************************************
Developer:   Michael Zuskin -  http://linkedin.com/in/zuskin | https://github.com/Ursego
**********************************************************************************************************************/
string      ls_source
string      ls_err
string      ls_ver
DataStore   lds

ls_ver = this.uf_get_pb_version()
ls_source = 'release ' + ls_ver + '; datawindow() table(column=(type=decimal(0) name=the_col dbname="the_col") )'

lds = create DataStore
lds.create(ls_source, ls_err)

if UpperBound(al_arr) > 0 then
   lds.object.the_col.current = al_arr
end if

return lds
