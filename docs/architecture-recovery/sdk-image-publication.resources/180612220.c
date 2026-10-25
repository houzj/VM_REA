
void FUN_180612220(longlong param_1)

{
  longlong lVar1;
  ulonglong uVar2;
  uint uVar3;
  ulonglong uVar5;
  ulonglong uVar6;
  ulonglong uVar4;
  
  if (*(longlong *)(param_1 + 8) != 0) {
    uVar6 = 0;
    uVar2 = uVar6;
    uVar5 = uVar6;
    if (0 < *(int *)(param_1 + 4)) {
      do {
        lVar1 = *(longlong *)(param_1 + 8);
        if ((*(int *)(uVar5 + lVar1) == 2) && (*(longlong *)(uVar5 + 0x108 + lVar1) != 0)) {
          uVar4 = uVar6;
          if (0 < *(int *)(uVar5 + 0x104 + lVar1)) {
            do {
              FUN_1806121a0((longlong)(int)uVar4 * 0x310 + *(longlong *)(uVar5 + 0x108 + lVar1));
              uVar3 = (int)uVar4 + 1;
              uVar4 = (ulonglong)uVar3;
            } while ((int)uVar3 < *(int *)(uVar5 + 0x104 + lVar1));
          }
          ___V_YAXPEAX_Z(*(undefined8 *)(uVar5 + 0x108 + lVar1));
          *(undefined8 *)(uVar5 + 0x108 + lVar1) = 0;
        }
        uVar3 = (int)uVar2 + 1;
        uVar2 = (ulonglong)uVar3;
        uVar5 = uVar5 + 0x310;
      } while ((int)uVar3 < *(int *)(param_1 + 4));
    }
    ___V_YAXPEAX_Z(*(undefined8 *)(param_1 + 8));
    *(undefined8 *)(param_1 + 8) = 0;
  }
  return;
}

