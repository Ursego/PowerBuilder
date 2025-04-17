// Use one compact CHOOSE CASE TRUE block to "pack" a series of Boolean expressions.

// For example, instead of

if not IsValid(ads) then
   ls_err = "Passed datastore is not valid"
end if

if ls_err = "" and IsNull(al_row) then
   ls_err = "Passed row cannot be null"
end if

if ls_err = "" and al_row < 1 then
   ls_err = "Passed row must be greater than 0"
end if

// write

choose case true
case not IsValid(ads)
   ls_err = "Passed datastore is not valid"
case IsNull(al_row)
   ls_err = "Passed row cannot be null"
case al_row < 1
   ls_err = "Passed row must be greater than 0"
end choose