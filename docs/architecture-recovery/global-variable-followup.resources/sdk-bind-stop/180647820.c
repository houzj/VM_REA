
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

int IMVS_ContinousRunStart_V30(longlong param_1,undefined4 param_2)

{
  undefined8 *puVar1;
  longlong lVar2;
  int iVar3;
  longlong *plVar4;
  undefined1 auStack_18 [16];
  
                    /* 0x647820  407  IMVS_ContinousRunStart_V30 */
  plVar4 = (longlong *)FUN_180645170();
  if (plVar4 != (longlong *)0x0) {
    for (puVar1 = *(undefined8 **)*plVar4; puVar1 != (undefined8 *)*plVar4;
        puVar1 = (undefined8 *)*puVar1) {
      if (puVar1[2] == param_1) {
        lVar2 = puVar1[3];
        if (lVar2 == 0) {
          return -0x1ffffffa;
        }
        FUN_180001070(auStack_18,lVar2);
        puVar1 = *(undefined8 **)*plVar4;
        goto joined_r0x000180647898;
      }
    }
  }
  return -0x1ffffffa;
joined_r0x000180647898:
  if (puVar1 == (undefined8 *)*plVar4) {
LAB_1806478ae:
    iVar3 = -0x1ffffffa;
LAB_1806478b3:
    func_0x0001800010b0(auStack_18);
    return iVar3;
  }
  if (puVar1[3] == lVar2) {
    if ((puVar1[2] != 0) && (puVar1[2] == param_1)) {
      if ((_DAT_180ca02d8 & 1) == 0) {
        _DAT_180ca02d8 = _DAT_180ca02d8 | 1;
        FUN_180660680();
        FUN_1809e7f58(&UNK_180a2a8b0);
      }
      iVar3 = FUN_180670170(&DAT_180ca02e0,param_2);
      if (iVar3 == 0) {
        func_0x0001800010b0(auStack_18);
        return 0;
      }
      goto LAB_1806478b3;
    }
    goto LAB_1806478ae;
  }
  puVar1 = (undefined8 *)*puVar1;
  goto joined_r0x000180647898;
}

