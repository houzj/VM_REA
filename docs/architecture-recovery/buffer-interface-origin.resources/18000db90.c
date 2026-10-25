
/* public: virtual __cdecl CImageBufferModule::~CImageBufferModule(void) __ptr64 */

void __thiscall CImageBufferModule::~CImageBufferModule(CImageBufferModule *this)

{
  longlong lVar1;
  longlong *plVar2;
  longlong *plVar3;
  
                    /* 0xdb90  3  ??1CImageBufferModule@@UEAA@XZ */
  *(undefined ***)this = vftable_for_CVmLogicModuleBase_;
  *(undefined ***)(this + 0x50) = vftable_for_CModuleSharedMemoryBase_;
  lVar1 = *(longlong *)(this + 0xd8);
  while (lVar1 != 0) {
    plVar2 = (longlong *)**(undefined8 **)(this + 0xd0);
    *(longlong *)plVar2[1] = *plVar2;
    *(longlong *)(*plVar2 + 8) = plVar2[1];
    *(longlong *)(this + 0xd8) = *(longlong *)(this + 0xd8) + -1;
    operator_delete(plVar2);
    lVar1 = *(longlong *)(this + 0xd8);
  }
  plVar2 = *(longlong **)(this + 0xd0);
  plVar3 = (longlong *)*plVar2;
  *plVar2 = (longlong)plVar2;
  *(longlong *)(*(longlong *)(this + 0xd0) + 8) = *(longlong *)(this + 0xd0);
  *(undefined8 *)(this + 0xd8) = 0;
  if (plVar3 != *(longlong **)(this + 0xd0)) {
    do {
      plVar2 = (longlong *)*plVar3;
      operator_delete(plVar3);
      plVar3 = plVar2;
    } while (plVar2 != (longlong *)*(longlong *)(this + 0xd0));
  }
  operator_delete(*(void **)(this + 0xd0));
  FUN_180007de0((longlong *)(this + 0x50));
  FUN_18000b0a0((CAbstractUserModule *)this);
  return;
}

