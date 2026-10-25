
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_1800d5d20(undefined8 param_1,longlong param_2,undefined4 param_3,longlong param_4,
                  int param_5,longlong param_6)

{
  int iVar1;
  int *piVar2;
  longlong lVar3;
  char cVar4;
  longlong lVar5;
  undefined8 uVar6;
  longlong lVar7;
  undefined1 auStack_b8 [32];
  longlong lStack_98;
  undefined8 uStack_88;
  char acStack_78 [64];
  ulonglong uStack_38;
  longlong lVar8;
  
  uStack_88 = 0xfffffffffffffffe;
  uStack_38 = _DAT_1802a5230 ^ (ulonglong)auStack_b8;
  piVar2 = *(int **)(param_2 + 0x28);
  if (piVar2 == (int *)0x0) {
LAB_1800d5d91:
    func_0x0001801a3b90(param_2 + 0x18,*(int *)(param_2 + 0x24) + 1);
LAB_1800d5d9e:
    **(int **)(param_2 + 0x28) = **(int **)(param_2 + 0x28) + 1;
    lVar5 = func_0x00018000f640();
    *(longlong *)(*(longlong *)(param_2 + 0x28) + 8 + (longlong)*(int *)(param_2 + 0x20) * 8) =
         lVar5;
    *(int *)(param_2 + 0x20) = *(int *)(param_2 + 0x20) + 1;
  }
  else {
    iVar1 = *(int *)(param_2 + 0x20);
    if (*piVar2 <= iVar1) {
      if ((piVar2 == (int *)0x0) || (*piVar2 == *(int *)(param_2 + 0x24))) goto LAB_1800d5d91;
      goto LAB_1800d5d9e;
    }
    lVar5 = *(longlong *)(piVar2 + (longlong)iVar1 * 2 + 2);
    *(int *)(param_2 + 0x20) = iVar1 + 1;
  }
  if (lVar5 == 0) {
    cVar4 = FUN_18001dbc0(4);
    if (cVar4 != '\0') {
      uVar6 = FUN_18001db50();
      lStack_98 = CONCAT44(lStack_98._4_4_,0x238);
      FUN_180015b40(uVar6,4,&UNK_18022ac60,&UNK_18022c178);
    }
    goto LAB_1800d5fd8;
  }
  *(uint *)(lVar5 + 0x10) = *(uint *)(lVar5 + 0x10) | 1;
  *(undefined4 *)(lVar5 + 0x30) = param_3;
  if (_DAT_1802b12a0 != 0) {
    if (param_6 == 0) {
      cVar4 = FUN_18001dbc0(4);
      if (cVar4 != '\0') {
        uVar6 = FUN_18001db50();
        lStack_98 = CONCAT44(lStack_98._4_4_,0x24b);
        FUN_180015b40(uVar6,4,&UNK_18022c1e8,&UNK_18022c178);
      }
      goto LAB_1800d5fd8;
    }
    acStack_78[0] = '\0';
    acStack_78[1] = '\0';
    acStack_78[2] = '\0';
    acStack_78[3] = '\0';
    acStack_78[4] = '\0';
    acStack_78[5] = '\0';
    acStack_78[6] = '\0';
    acStack_78[7] = '\0';
    acStack_78[8] = '\0';
    acStack_78[9] = '\0';
    acStack_78[10] = '\0';
    acStack_78[0xb] = '\0';
    acStack_78[0xc] = '\0';
    acStack_78[0xd] = '\0';
    acStack_78[0xe] = '\0';
    acStack_78[0xf] = '\0';
    acStack_78[0x10] = '\0';
    acStack_78[0x11] = '\0';
    acStack_78[0x12] = '\0';
    acStack_78[0x13] = '\0';
    acStack_78[0x14] = '\0';
    acStack_78[0x15] = '\0';
    acStack_78[0x16] = '\0';
    acStack_78[0x17] = '\0';
    acStack_78[0x18] = '\0';
    acStack_78[0x19] = '\0';
    acStack_78[0x1a] = '\0';
    acStack_78[0x1b] = '\0';
    acStack_78[0x1c] = '\0';
    acStack_78[0x1d] = '\0';
    acStack_78[0x1e] = '\0';
    acStack_78[0x1f] = '\0';
    acStack_78[0x20] = '\0';
    acStack_78[0x21] = '\0';
    acStack_78[0x22] = '\0';
    acStack_78[0x23] = '\0';
    acStack_78[0x24] = '\0';
    acStack_78[0x25] = '\0';
    acStack_78[0x26] = '\0';
    acStack_78[0x27] = '\0';
    acStack_78[0x28] = '\0';
    acStack_78[0x29] = '\0';
    acStack_78[0x2a] = '\0';
    acStack_78[0x2b] = '\0';
    acStack_78[0x2c] = '\0';
    acStack_78[0x2d] = '\0';
    acStack_78[0x2e] = '\0';
    acStack_78[0x2f] = '\0';
    acStack_78[0x30] = '\0';
    acStack_78[0x31] = '\0';
    acStack_78[0x32] = '\0';
    acStack_78[0x33] = '\0';
    acStack_78[0x34] = '\0';
    acStack_78[0x35] = '\0';
    acStack_78[0x36] = '\0';
    acStack_78[0x37] = '\0';
    acStack_78[0x38] = '\0';
    acStack_78[0x39] = '\0';
    acStack_78[0x3a] = '\0';
    acStack_78[0x3b] = '\0';
    acStack_78[0x3c] = '\0';
    acStack_78[0x3d] = '\0';
    acStack_78[0x3e] = '\0';
    acStack_78[0x3f] = 0;
    lStack_98 = param_6;
    _snprintf_s(acStack_78,0x3f,0xffffffffffffffff,&UNK_18022c250);
    lVar8 = -1;
    do {
      lVar7 = lVar8 + 1;
      lVar3 = lVar8 + 1;
      lVar8 = lVar7;
    } while (acStack_78[lVar3] != '\0');
    piVar2 = *(int **)(lVar5 + 0x28);
    if (piVar2 == (int *)0x0) {
LAB_1800d5f9a:
      func_0x0001801a3b90(lVar5 + 0x18,*(int *)(lVar5 + 0x24) + 1);
LAB_1800d5fa8:
      **(int **)(lVar5 + 0x28) = **(int **)(lVar5 + 0x28) + 1;
      uVar6 = FUN_18000ddc0(*(undefined8 *)(lVar5 + 0x18));
      *(undefined8 *)(*(longlong *)(lVar5 + 0x28) + 8 + (longlong)*(int *)(lVar5 + 0x20) * 8) =
           uVar6;
      *(int *)(lVar5 + 0x20) = *(int *)(lVar5 + 0x20) + 1;
    }
    else {
      iVar1 = *(int *)(lVar5 + 0x20);
      if (*piVar2 <= iVar1) {
        if ((piVar2 == (int *)0x0) || (*piVar2 == *(int *)(lVar5 + 0x24))) goto LAB_1800d5f9a;
        goto LAB_1800d5fa8;
      }
      uVar6 = *(undefined8 *)(piVar2 + (longlong)iVar1 * 2 + 2);
      *(int *)(lVar5 + 0x20) = iVar1 + 1;
    }
    FUN_180006300(uVar6,acStack_78,lVar7);
    goto LAB_1800d5fd8;
  }
  if (param_4 == 0) {
    cVar4 = FUN_18001dbc0(4);
    if (cVar4 != '\0') {
      uVar6 = FUN_18001db50();
      lStack_98 = CONCAT44(lStack_98._4_4_,0x241);
      FUN_180015b40(uVar6,4,&UNK_18022c208,&UNK_18022c178);
    }
    goto LAB_1800d5fd8;
  }
  piVar2 = *(int **)(lVar5 + 0x28);
  if (piVar2 == (int *)0x0) {
LAB_1800d5e82:
    func_0x0001801a3b90(lVar5 + 0x18,*(int *)(lVar5 + 0x24) + 1);
LAB_1800d5e90:
    **(int **)(lVar5 + 0x28) = **(int **)(lVar5 + 0x28) + 1;
    uVar6 = FUN_18000ddc0(*(undefined8 *)(lVar5 + 0x18));
    *(undefined8 *)(*(longlong *)(lVar5 + 0x28) + 8 + (longlong)*(int *)(lVar5 + 0x20) * 8) = uVar6;
    *(int *)(lVar5 + 0x20) = *(int *)(lVar5 + 0x20) + 1;
  }
  else {
    iVar1 = *(int *)(lVar5 + 0x20);
    if (*piVar2 <= iVar1) {
      if ((piVar2 == (int *)0x0) || (*piVar2 == *(int *)(lVar5 + 0x24))) goto LAB_1800d5e82;
      goto LAB_1800d5e90;
    }
    uVar6 = *(undefined8 *)(piVar2 + (longlong)iVar1 * 2 + 2);
    *(int *)(lVar5 + 0x20) = iVar1 + 1;
  }
  FUN_180006300(uVar6,param_4,(longlong)param_5);
LAB_1800d5fd8:
  FUN_1801e8810(uStack_38 ^ (ulonglong)auStack_b8);
  return;
}

