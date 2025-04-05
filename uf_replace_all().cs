// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// uf_replace_all() replaces all occurrences of a fragment within a string with another fragment.
// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// Before creating this function, make sure a similar function doesn't already exists in your framework.
// For example, PFC has n_cst_string.of_GlobalReplace().

/**********************************************************************************************************************
Dscr:       Replaces all occurrences of a fragment within a string with another fragment.
***********************************************************************************************************************
Arg:        rs_processed_string (ref) - the string in which replacement should take place.
            as_old_frag - the fragment to be replaced.
            as_new_frag - the fragment to replace with.
***********************************************************************************************************************
Developer:  Michael Zuskin - http://linkedin.com/in/zuskin | https://github.com/Ursego/
**********************************************************************************************************************/
long ll_pos
long ll_old_frag_len
long ll_new_frag_len

ll_old_frag_len = Len(as_old_frag)
ll_new_frag_len = Len(as_new_frag)

ll_pos = Pos(rs_processed_string, as_old_frag)
do while ll_pos > 0
   rs_processed_string = Replace(rs_processed_string, ll_pos, ll_old_frag_len, as_new_frag)
   ll_pos = Pos(rs_processed_string, as_old_frag, ll_pos + ll_new_frag_len)
loop

return
