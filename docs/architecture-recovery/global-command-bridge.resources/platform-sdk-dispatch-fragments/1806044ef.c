
void FUN_1806044ef(undefined8 param_1)

{
  char cVar1;
  longlong lVar2;
  undefined8 uVar3;
  undefined4 *puVar4;
  undefined8 uVar5;
  longlong unaff_RBP;
  longlong unaff_R14;
  ulonglong uVar6;
  undefined8 uVar7;
  int in_stack_00000040;
  longlong in_stack_00000048;
  undefined8 in_stack_00000050;
  
  FUN_180619670(param_1,&stack0x00000070);
  uVar6 = 0xffffffffffffffff;
  lVar2 = func_0x00018058c660();
  if ((ulonglong)(lVar2 - in_stack_00000048) < 0xc9) {
    do {
      uVar6 = uVar6 + 1;
    } while (*(char *)(unaff_R14 + uVar6) != '\0');
    if (0x7f < uVar6) goto LAB_180615fa8;
    cVar1 = FUN_18057a5b0(1);
    if (cVar1 == '\0') goto LAB_180615fa8;
    uVar3 = FUN_18057a310();
    uVar7 = CONCAT44(in_stack_00000050._4_4_,0x672);
    uVar5 = 1;
  }
  else {
    cVar1 = FUN_18057a5b0(2);
    if (cVar1 == '\0') goto LAB_180615fa8;
    uVar3 = FUN_18057a310();
    uVar7 = CONCAT44(in_stack_00000050._4_4_,0x66c);
    uVar5 = 2;
  }
  FUN_18057a4b0(uVar3,uVar5,&UNK_1807de1c0,&UNK_1807ddeb8,uVar7);
  in_stack_00000050._4_4_ = (undefined4)((ulonglong)uVar7 >> 0x20);
LAB_180615fa8:
  if (in_stack_00000040 == 0x44e) {
    cVar1 = FUN_18057a5b0(3);
    if (cVar1 != '\0') {
      uVar3 = FUN_18057a310();
      uVar5 = CONCAT44(in_stack_00000050._4_4_,0x678);
      FUN_18057a4b0(uVar3,3,&UNK_1807de1a0,&UNK_1807ddeb8,uVar5);
      in_stack_00000050._4_4_ = (undefined4)((ulonglong)uVar5 >> 0x20);
    }
    puVar4 = (undefined4 *)FUN_1806a92b0();
    *puVar4 = 0;
    func_0x0001806a9310(puVar4);
  }
  cVar1 = FUN_18057a5b0(1);
  if (cVar1 != '\0') {
    uVar3 = FUN_18057a310();
    FUN_18057a4b0(uVar3,1,&UNK_1807de280,&UNK_1807ddeb8,CONCAT44(in_stack_00000050._4_4_,0x67c));
  }
  FUN_180577d70(unaff_RBP + 0x17a0);
  func_0x00018059f040(&stack0x00000070);
  FUN_180745b60(*(ulonglong *)(unaff_RBP + 0x1930) ^ (ulonglong)&stack0x00000000);
  return;
}

