
void FUN_18060d99d(undefined8 param_1)

{
  char cVar1;
  undefined8 uVar2;
  longlong lVar3;
  undefined4 *puVar4;
  undefined8 uVar5;
  int unaff_EBX;
  longlong unaff_RBP;
  undefined4 unaff_R13D;
  longlong unaff_R14;
  ulonglong uVar6;
  undefined8 in_stack_00000020;
  undefined4 uVar8;
  undefined8 uVar7;
  int in_stack_00000040;
  longlong in_stack_00000048;
  longlong *in_stack_00000050;
  undefined8 in_stack_00000060;
  longlong in_stack_00000068;
  
  uVar8 = (undefined4)((ulonglong)in_stack_00000020 >> 0x20);
  if (unaff_EBX == 0) {
    lVar3 = unaff_R14;
    FUN_18062b200(param_1,in_stack_00000060,unaff_R13D,&stack0x00000070);
    uVar8 = (undefined4)((ulonglong)lVar3 >> 0x20);
    *(undefined1 *)(in_stack_00000068 + 0x7fc20) = 1;
  }
  else {
    cVar1 = FUN_18057a5b0(4);
    if (cVar1 != '\0') {
      uVar2 = FUN_18057a310();
      uVar5 = CONCAT44(uVar8,0x452);
      FUN_18057a4b0(uVar2,4,&UNK_1807ddf20,&UNK_1807ddeb8,uVar5);
      uVar8 = (undefined4)((ulonglong)uVar5 >> 0x20);
    }
    _snprintf();
    lVar3 = -1;
    do {
      lVar3 = lVar3 + 1;
    } while (*(char *)(unaff_R14 + lVar3) != '\0');
    *in_stack_00000050 = lVar3;
  }
  uVar6 = 0xffffffffffffffff;
  lVar3 = func_0x00018058c660();
  if ((ulonglong)(lVar3 - in_stack_00000048) < 0xc9) {
    do {
      uVar6 = uVar6 + 1;
    } while (*(char *)(unaff_R14 + uVar6) != '\0');
    if ((0x7f < uVar6) || (cVar1 = FUN_18057a5b0(1), cVar1 == '\0')) goto LAB_180615fa8;
    uVar2 = FUN_18057a310();
    uVar7 = CONCAT44(uVar8,0x672);
    uVar5 = 1;
  }
  else {
    cVar1 = FUN_18057a5b0(2);
    if (cVar1 == '\0') goto LAB_180615fa8;
    uVar2 = FUN_18057a310();
    uVar7 = CONCAT44(uVar8,0x66c);
    uVar5 = 2;
  }
  FUN_18057a4b0(uVar2,uVar5,&UNK_1807de1c0,&UNK_1807ddeb8,uVar7);
  uVar8 = (undefined4)((ulonglong)uVar7 >> 0x20);
LAB_180615fa8:
  if (in_stack_00000040 == 0x44e) {
    cVar1 = FUN_18057a5b0(3);
    if (cVar1 != '\0') {
      uVar2 = FUN_18057a310();
      uVar5 = CONCAT44(uVar8,0x678);
      FUN_18057a4b0(uVar2,3,&UNK_1807de1a0,&UNK_1807ddeb8,uVar5);
      uVar8 = (undefined4)((ulonglong)uVar5 >> 0x20);
    }
    puVar4 = (undefined4 *)FUN_1806a92b0();
    *puVar4 = 0;
    func_0x0001806a9310(puVar4);
  }
  cVar1 = FUN_18057a5b0(1);
  if (cVar1 != '\0') {
    uVar2 = FUN_18057a310();
    FUN_18057a4b0(uVar2,1,&UNK_1807de280,&UNK_1807ddeb8,CONCAT44(uVar8,0x67c));
  }
  FUN_180577d70(unaff_RBP + 0x17a0);
  func_0x00018059f040(&stack0x00000070);
  FUN_180745b60(*(ulonglong *)(unaff_RBP + 0x1930) ^ (ulonglong)&stack0x00000000);
  return;
}

