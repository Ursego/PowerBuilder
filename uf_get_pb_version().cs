// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_get_pb_version() reports the major PowerBuilder's version
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// For example, the function can be useful when you’re dynamically building a DW's source code that have the "release" field:
// https://github.com/Ursego/PowerBuilder/blob/main/uf_ds_from_array().cs

/**********************************************************************************************************************
Dscr:        Returns the major PowerBuilder's version, like "12" (not "12.5")
***********************************************************************************************************************
Ret:         string
***********************************************************************************************************************
Developer:   Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego
**********************************************************************************************************************/
int         li_rc
environment lenv

li_rc = GetEnvironment(ref lenv)
if li_rc <> 1 then
   MessageBox(this.ClassName() + ".uf_get_pb_version", "GetEnvironment() failed.", StopSign!)
   return ""
end if

return String(lenv.pbmajorrevision)