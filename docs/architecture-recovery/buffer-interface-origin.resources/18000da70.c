
/* public: __cdecl CImageBufferModule::CImageBufferModule(void) __ptr64 */

CImageBufferModule * __thiscall CImageBufferModule::CImageBufferModule(CImageBufferModule *this)

{
  CImageBufferModule *pCVar1;
  undefined8 uVar2;
  CImageBufferModule *pCVar3;
  CImageBufferModule *local_res10;
  
                    /* 0xda70  2  ??0CImageBufferModule@@QEAA@XZ */
  CAbstractUserModule::CAbstractUserModule((CAbstractUserModule *)this);
  *(undefined ***)this = CVmLogicModuleBase::vftable;
  *(undefined8 *)(this + 0x30) = 0;
  *(undefined8 *)(this + 0x38) = 0;
  *(undefined8 *)(this + 0x40) = 0;
  *(undefined8 *)(this + 0x10) = 0;
  *(undefined4 *)(this + 0x18) = 0;
  *(undefined8 *)(this + 0x20) = 0;
  *(undefined4 *)(this + 0x28) = 0;
  *(undefined4 *)(this + 0x48) = 0xffffffff;
  pCVar1 = this + 0x50;
  *(undefined ***)pCVar1 = CModuleSharedMemoryBase::vftable;
  *(undefined4 *)(this + 0x58) = 1;
  *(undefined8 *)(this + 0xb0) = 0;
  *(undefined8 *)(this + 0xb8) = 0;
  local_res10 = pCVar1;
  uVar2 = FUN_18000a620();
  *(undefined8 *)(this + 0xb0) = uVar2;
  *(undefined8 *)(this + 0xc0) = 0;
  *(undefined8 *)(this + 200) = 0;
  uVar2 = FUN_18000a670();
  *(undefined8 *)(this + 0xc0) = uVar2;
  pCVar3 = pCVar1;
  FUN_180009260((longlong)pCVar1);
  *(undefined ***)this = vftable_for_CVmLogicModuleBase_;
  *(undefined ***)pCVar1 = vftable_for_CModuleSharedMemoryBase_;
  *(undefined8 *)(this + 0xd0) = 0;
  *(undefined8 *)(this + 0xd8) = 0;
  uVar2 = FUN_18000f190(pCVar3,(undefined8 *)0x0,(undefined8 *)0x0);
  *(undefined8 *)(this + 0xd0) = uVar2;
  *(undefined4 *)(this + 0xe0) = 1;
  *(undefined4 *)(this + 0x48) = 0;
  local_res10 = (CImageBufferModule *)0x0;
  (**(code **)(*(longlong *)pCVar1 + 0x20))(pCVar1,&local_res10,0);
  sscanf_s((char *)local_res10,"%d",this + 0xe0);
  return this;
}

