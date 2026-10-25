
/* public: virtual int __cdecl CImageBufferModule::DeInit(void) __ptr64 */

int __thiscall CImageBufferModule::DeInit(CImageBufferModule *this)

{
  longlong lVar1;
  longlong *plVar2;
  
                    /* 0xdd20  8  ?DeInit@CImageBufferModule@@UEAAHXZ */
  lVar1 = *(longlong *)(this + 0xd8);
  while (lVar1 != 0) {
    plVar2 = (longlong *)**(undefined8 **)(this + 0xd0);
    *(longlong *)plVar2[1] = *plVar2;
    *(longlong *)(*plVar2 + 8) = plVar2[1];
    *(longlong *)(this + 0xd8) = *(longlong *)(this + 0xd8) + -1;
    operator_delete(plVar2);
    lVar1 = *(longlong *)(this + 0xd8);
  }
  return 0;
}

