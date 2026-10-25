
/* public: virtual int __cdecl CImageBufferModule::Init(void) __ptr64 */

int __thiscall CImageBufferModule::Init(CImageBufferModule *this)

{
  int iVar1;
  undefined8 in_R8;
  undefined8 in_R9;
  
                    /* 0xdc80  12  ?Init@CImageBufferModule@@UEAAHXZ */
  iVar1 = VM_M_GetModuleId(*(undefined8 *)(this + 8),this + 0x48,in_R8,in_R9,0xfffffffffffffffe);
  if (iVar1 == 0) {
    (**(code **)(*(longlong *)this + 0x38))(this);
    SetModuleNodeNum(this);
    iVar1 = 0;
  }
  return iVar1;
}

