
void FUN_1800a7190(longlong param_1,undefined4 param_2,undefined4 param_3,undefined4 param_4)

{
  longlong *plVar1;
  int *piVar2;
  longlong *plVar3;
  undefined4 uVar4;
  undefined8 uVar5;
  undefined1 auStack_118 [32];
  longlong lStack_f8;
  longlong *plStack_f0;
  longlong lStack_e8;
  longlong *plStack_e0;
  longlong lStack_d8;
  longlong *plStack_d0;
  longlong lStack_c8;
  longlong *plStack_c0;
  longlong lStack_b8;
  longlong *plStack_b0;
  longlong lStack_a8;
  longlong *plStack_a0;
  undefined8 uStack_98;
  undefined1 uStack_90;
  undefined7 uStack_8f;
  undefined8 uStack_80;
  ulonglong uStack_78;
  undefined1 uStack_70;
  undefined7 uStack_6f;
  undefined8 uStack_60;
  ulonglong uStack_58;
  undefined1 uStack_50;
  undefined7 uStack_4f;
  undefined8 uStack_40;
  ulonglong uStack_38;
  ulonglong uStack_30;
  
  uStack_98 = 0xfffffffffffffffe;
  uStack_30 = DAT_1802a5230 ^ (ulonglong)auStack_118;
  uStack_78 = 0xf;
  uStack_80 = 0;
  uStack_90 = 0;
  FUN_180006300(&uStack_90,&DAT_180227688,10);
  uVar4 = *(undefined4 *)(param_1 + 0x2c);
  uVar5 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
  FUN_1800cae00(uVar5,&lStack_f8,uVar4);
  if (lStack_f8 != 0) {
    FUN_1800f74b0(lStack_f8,&lStack_c8,&uStack_90);
    if (lStack_c8 != 0) {
      FUN_1800f3ee0(lStack_c8,0,param_2);
    }
    if (plStack_c0 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_c0 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_c0)(plStack_c0);
        LOCK();
        piVar2 = (int *)((longlong)plStack_c0 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_c0 + 8))(plStack_c0);
        }
      }
    }
  }
  plVar1 = plStack_f0;
  if (plStack_f0 != (longlong *)0x0) {
    LOCK();
    plVar3 = plStack_f0 + 1;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)*plVar3 == 0) {
      (**(code **)*plStack_f0)(plStack_f0);
      LOCK();
      piVar2 = (int *)((longlong)plVar1 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_f0 + 8))();
      }
    }
  }
  if (0xf < uStack_78) {
    __3_YAXPEAX_Z(CONCAT71(uStack_8f,uStack_90));
  }
  uStack_58 = 0xf;
  uStack_60 = 0;
  uStack_70 = 0;
  FUN_180006300(&uStack_70,&DAT_180227748,8);
  uVar4 = *(undefined4 *)(param_1 + 0x2c);
  uVar5 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
  FUN_1800cae00(uVar5,&lStack_d8,uVar4);
  if (lStack_d8 != 0) {
    FUN_1800f74b0(lStack_d8,&lStack_a8,&uStack_70);
    if (lStack_a8 != 0) {
      FUN_1800f3ee0(lStack_a8,0,param_3);
    }
    if (plStack_a0 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_a0 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_a0)(plStack_a0);
        LOCK();
        piVar2 = (int *)((longlong)plStack_a0 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_a0 + 8))(plStack_a0);
        }
      }
    }
  }
  plVar1 = plStack_d0;
  if (plStack_d0 != (longlong *)0x0) {
    LOCK();
    plVar3 = plStack_d0 + 1;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)*plVar3 == 0) {
      (**(code **)*plStack_d0)(plStack_d0);
      LOCK();
      piVar2 = (int *)((longlong)plVar1 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_d0 + 8))();
      }
    }
  }
  if (0xf < uStack_58) {
    __3_YAXPEAX_Z(CONCAT71(uStack_6f,uStack_70));
  }
  uStack_38 = 0xf;
  uStack_40 = 0;
  uStack_50 = 0;
  FUN_180006300(&uStack_50,&DAT_180227738,9);
  uVar4 = *(undefined4 *)(param_1 + 0x2c);
  uVar5 = FUN_18008e6c0(*(undefined8 *)(param_1 + 0xf8));
  FUN_1800cae00(uVar5,&lStack_e8,uVar4);
  if (lStack_e8 != 0) {
    FUN_1800f74b0(lStack_e8,&lStack_b8,&uStack_50);
    if (lStack_b8 != 0) {
      FUN_1800f3ee0(lStack_b8,0,param_4);
    }
    if (plStack_b0 != (longlong *)0x0) {
      LOCK();
      plVar1 = plStack_b0 + 1;
      *(int *)plVar1 = (int)*plVar1 + -1;
      UNLOCK();
      if ((int)*plVar1 == 0) {
        (**(code **)*plStack_b0)(plStack_b0);
        LOCK();
        piVar2 = (int *)((longlong)plStack_b0 + 0xc);
        *piVar2 = *piVar2 + -1;
        UNLOCK();
        if (*piVar2 == 0) {
          (**(code **)(*plStack_b0 + 8))(plStack_b0);
        }
      }
    }
  }
  plVar1 = plStack_e0;
  if (plStack_e0 != (longlong *)0x0) {
    LOCK();
    plVar3 = plStack_e0 + 1;
    *(int *)plVar3 = (int)*plVar3 + -1;
    UNLOCK();
    if ((int)*plVar3 == 0) {
      (**(code **)*plStack_e0)(plStack_e0);
      LOCK();
      piVar2 = (int *)((longlong)plVar1 + 0xc);
      *piVar2 = *piVar2 + -1;
      UNLOCK();
      if (*piVar2 == 0) {
        (**(code **)(*plStack_e0 + 8))();
      }
    }
  }
  if (0xf < uStack_38) {
    __3_YAXPEAX_Z(CONCAT71(uStack_4f,uStack_50));
  }
  FUN_1801e8810(uStack_30 ^ (ulonglong)auStack_118);
  return;
}

