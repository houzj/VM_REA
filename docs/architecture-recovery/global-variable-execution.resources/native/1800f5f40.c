
undefined4
FUN_1800f5f40(longlong *param_1,undefined8 param_2,int param_3,undefined4 *param_4,int *param_5)

{
  int *piVar1;
  longlong lVar2;
  int iVar3;
  longlong lVar4;
  longlong *plVar5;
  undefined4 uVar6;
  longlong lStack_68;
  undefined4 uStack_60;
  longlong lStack_58;
  longlong lStack_50;
  longlong *plStack_48;
  undefined8 uStack_40;
  
  uStack_40 = 0xfffffffffffffffe;
  uVar6 = 0;
  uStack_60 = 0;
  lVar2 = *param_1;
  lVar4 = FUN_1800202d0();
  if ((lVar4 == *param_1) || (iVar3 = FUN_180020a60(param_2,lVar4 + 0x20), iVar3 < 0)) {
    lStack_68 = *param_1;
    plVar5 = &lStack_68;
  }
  else {
    lStack_58 = lVar4;
    plVar5 = &lStack_58;
  }
  if (*plVar5 == lVar2) {
    uVar6 = 0xe0000314;
  }
  else {
    FUN_1800f74b0(param_1,&lStack_50,param_2);
    if (lStack_50 == 0) {
      uVar6 = 0xe0000003;
    }
    else if (param_3 < 0) {
      uVar6 = 0xe0000014;
    }
    else {
      lVar2 = *(longlong *)(lStack_50 + 0x10);
      if (lVar2 == 0) {
        uVar6 = 0xe0000314;
      }
      else {
        iVar3 = *(int *)(lVar2 + 0x68);
        *param_5 = iVar3;
        if (param_3 < iVar3) {
          *param_4 = *(undefined4 *)(*(longlong *)(lVar2 + 0x70) + 8 + (longlong)param_3 * 4);
        }
        else {
          uVar6 = 0xe0000014;
          if (iVar3 == 0) {
            uVar6 = 0xe0000314;
          }
        }
      }
    }
    if (plStack_48 != (longlong *)0x0) {
      LOCK();
      plVar5 = plStack_48 + 1;
      *(int *)plVar5 = (int)*plVar5 + -1;
      UNLOCK();
      if ((int)*plVar5 == 0) {
        (**(code **)*plStack_48)(plStack_48);
        LOCK();
        piVar1 = (int *)((longlong)plStack_48 + 0xc);
        *piVar1 = *piVar1 + -1;
        UNLOCK();
        if (*piVar1 == 0) {
          (**(code **)(*plStack_48 + 8))(plStack_48);
        }
      }
    }
  }
  return uVar6;
}

