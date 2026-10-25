
/* private: void __cdecl CImageBufferModule::ResetMember(void) __ptr64 */

void __thiscall CImageBufferModule::ResetMember(CImageBufferModule *this)

{
  char *local_res8 [4];
  
                    /* 0xdcd0  16  ?ResetMember@CImageBufferModule@@AEAAXXZ */
  *(undefined4 *)(this + 0x48) = 0;
  local_res8[0] = (char *)0x0;
  (**(code **)(*(longlong *)(this + 0x50) + 0x20))(this + 0x50,local_res8,0);
  sscanf_s(local_res8[0],"%d");
  return;
}

