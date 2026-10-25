
undefined8 FUN_180107170(undefined8 param_1,int *param_2,undefined8 param_3,undefined8 param_4)

{
  int iVar1;
  undefined1 *puVar2;
  undefined4 uVar3;
  undefined8 uVar4;
  longlong alStack_108 [2];
  undefined1 auStack_f8 [8];
  undefined1 auStack_f0 [8];
  undefined1 auStack_e8 [120];
  undefined1 auStack_70 [104];
  
  uVar4 = 0xfffffffffffffffe;
  uVar3 = 0;
  FUN_180090200(alStack_108);
  FUN_180037640(auStack_f8,&UNK_180230470);
  iVar1 = *param_2;
  puVar2 = auStack_f8;
  if (iVar1 == 1) {
    puVar2 = (undefined1 *)FUN_180037640(puVar2,&UNK_1802304f8,0,param_4,uVar3,uVar4);
    param_2 = param_2 + 1;
  }
  else if (iVar1 == 2) {
    param_2 = (int *)&UNK_1802304d0;
  }
  else if (iVar1 == 3) {
    FUN_180037640(puVar2,&UNK_180230520,0,param_4,uVar3,uVar4);
    uVar4 = FUN_180037640(auStack_f8,&UNK_180230510);
    uVar4 = __6__basic_ostream_DU__char_traits_D_std___std__QEAAAEAV01_H_Z(uVar4,param_2[0x33]);
    uVar4 = FUN_180037640(uVar4,&UNK_180230490);
    uVar4 = FUN_180037640(uVar4,param_2 + 0x34);
    FUN_180037640(uVar4,&UNK_1802290c0);
    FUN_180037640(auStack_f8,&UNK_180230550);
    iVar1 = param_2[1];
    if (iVar1 == 1) {
      param_2 = (int *)&UNK_1802255bc;
      puVar2 = auStack_f8;
    }
    else {
      if (iVar1 == 2) {
        uVar4 = FUN_180037640(auStack_f8,&UNK_1802304f8);
        uVar4 = __6__basic_ostream_DU__char_traits_D_std___std__QEAAAEAV01_H_Z(uVar4,param_2[2]);
        goto LAB_1801072e3;
      }
      puVar2 = auStack_f8;
      if (iVar1 == 3) {
        uVar4 = FUN_180037640(puVar2,&UNK_180230538);
        uVar4 = __6__basic_ostream_DU__char_traits_D_std___std__QEAAAEAV01_H_Z(uVar4,param_2[2]);
        puVar2 = (undefined1 *)FUN_180037640(uVar4,&UNK_180230490);
        param_2 = param_2 + 3;
      }
      else {
        param_2 = (int *)&UNK_180230578;
      }
    }
  }
  else {
    param_2 = (int *)&UNK_180230568;
  }
  uVar4 = FUN_180037640(puVar2,param_2);
LAB_1801072e3:
  FUN_180037640(uVar4,&UNK_1802290c0);
  FUN_180037640(auStack_f8,&UNK_1802290c0);
  FUN_1800902b0(alStack_108,param_1);
  *(undefined **)((longlong)alStack_108 + (longlong)*(int *)(alStack_108[0] + 4)) = &UNK_180225bb8;
  *(int *)(auStack_f0 + (longlong)*(int *)(alStack_108[0] + 4) + -0x1c) =
       *(int *)(alStack_108[0] + 4) + -0x98;
  FUN_1800902e0(auStack_f0);
  __1__basic_iostream_DU__char_traits_D_std___std__UEAA_XZ(auStack_e8);
  __1__basic_ios_DU__char_traits_D_std___std__UEAA_XZ(auStack_70);
  return param_1;
}

