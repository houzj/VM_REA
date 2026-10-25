
void FUN_18004d2d1(void)

{
  char cVar1;
  undefined4 uVar2;
  int iVar3;
  undefined8 uVar4;
  undefined8 uVar5;
  undefined1 *puVar6;
  undefined1 *puVar7;
  undefined *puVar8;
  longlong unaff_R15;
  undefined4 in_stack_00000148;
  undefined4 in_stack_00000150;
  undefined4 in_stack_00000158;
  ulonglong in_stack_0001f880;
  
  cVar1 = FUN_180027d50(&stack0x00000db0,&UNK_18014f0dc);
  if (cVar1 == '\0') {
    cVar1 = FUN_18000c2c0(4);
    if (cVar1 != '\0') {
      uVar4 = FUN_18000bfe0();
      FUN_18000c1c0(uVar4,4,&UNK_18014f0e8,&UNK_18014efc8,0x42a);
    }
    in_stack_00000148 = 0xe0000109;
    func_0x00018011e41a(&stack0x00000148,&UNK_18018ea80);
  }
  uVar4 = FUN_180028310(&stack0x00000db0,&UNK_18014f0dc);
  uVar4 = FUN_180028310(uVar4,&UNK_18014f200);
  uVar2 = func_0x000180027d90(uVar4);
  func_0x00018011e414(&stack0x00002db4,0,0x98);
  strcpy_s(&stack0x00002db4,0x40,&UNK_18014b0d8);
  uVar4 = FUN_180021530();
  iVar3 = FUN_1800244d0(uVar4,uVar2);
  if (iVar3 == 0) {
    strcpy_s(&stack0x00002dfc,0x40,&UNK_18014b0e4);
  }
  else {
    strcpy_s(&stack0x00002dfc,0x40,&UNK_18014c9c8);
    cVar1 = FUN_18000c2c0(4);
    if (cVar1 != '\0') {
      uVar4 = FUN_18000bfe0();
      FUN_18000c1c0(uVar4,4,&UNK_18014f8b0,&UNK_18014efc8,0x43b);
    }
  }
  iVar3 = FUN_180061a60();
  if (iVar3 != 0) {
    cVar1 = FUN_18000c2c0(4);
    if (cVar1 != '\0') {
      uVar4 = FUN_18000bfe0();
      FUN_18000c1c0(uVar4,4,&UNK_18014f1c0,&UNK_18014efc8,0x446);
    }
    func_0x00018011e41a(&stack0x0000039c,&UNK_18018ea80);
    cVar1 = FUN_180027d50(&stack0x00000db0,&UNK_18014f0dc);
    if (cVar1 == '\0') {
      cVar1 = FUN_18000c2c0(4);
      if (cVar1 != '\0') {
        uVar4 = FUN_18000bfe0();
        FUN_18000c1c0(uVar4,4,&UNK_18014f0e8,&UNK_18014efc8,0x44f);
      }
      in_stack_00000150 = 0xe0000109;
      func_0x00018011e41a(&stack0x00000150,&UNK_18018ea80);
    }
    uVar4 = FUN_180028310(&stack0x00000db0,&UNK_18014f0dc);
    uVar4 = FUN_180028310(uVar4,&UNK_18014f268);
    uVar2 = func_0x000180027d90(uVar4);
    func_0x00018011e414(&stack0x00001d34,0,0x98);
    strcpy_s(&stack0x00001d34,0x40,&UNK_18014b0d8);
    if (*(int *)(unaff_R15 + 0x90) == 0) {
      uVar4 = FUN_180021530();
      iVar3 = func_0x0001800244f0(uVar4,uVar2);
      if (iVar3 == 0) {
        strcpy_s(&stack0x00001d7c,0x40,&UNK_18014b0e4);
      }
      else {
        strcpy_s(&stack0x00001d7c,0x40,&UNK_18014c9c8);
        cVar1 = FUN_18000c2c0(4);
        if (cVar1 != '\0') {
          uVar4 = FUN_18000bfe0();
          FUN_18000c1c0(uVar4,4,&UNK_18014f9a0,&UNK_18014efc8,0x462);
        }
      }
    }
    else {
      strcpy_s(&stack0x00001d7c,0x40,&UNK_18014c9c8);
      cVar1 = FUN_18000c2c0(4);
      if (cVar1 != '\0') {
        uVar4 = FUN_18000bfe0();
        FUN_18000c1c0(uVar4,4,&UNK_18014f9e8,&UNK_18014efc8,0x46e);
      }
    }
    iVar3 = FUN_180061a60();
    if (iVar3 != 0) {
      cVar1 = FUN_18000c2c0(4);
      if (cVar1 != '\0') {
        uVar4 = FUN_18000bfe0();
        FUN_18000c1c0(uVar4,4,&UNK_18014f1c0,&UNK_18014efc8,0x474);
      }
      func_0x00018011e41a(&stack0x000004ac,&UNK_18018ea80);
      cVar1 = FUN_18000c2c0(2);
      if (cVar1 != '\0') {
        uVar4 = FUN_18000bfe0();
        FUN_18000c1c0(uVar4,2,&UNK_18014f958,&UNK_18014efc8,0x47b);
      }
      FUN_1800115c0(&stack0x00000f28,unaff_R15 + 0xe8,&UNK_18014f990);
      FUN_180018800();
      func_0x0001800095f0(&stack0x00000f28);
      uVar4 = FUN_18000cdc0();
      uVar4 = func_0x00018000ff30(uVar4);
      iVar3 = _wmkdir(uVar4);
      FUN_18000fe50(&stack0x00000798);
      if (iVar3 != 0) {
        cVar1 = FUN_18000c2c0(3);
        if (cVar1 != '\0') {
          _errno();
          uVar4 = FUN_18000bfe0();
          FUN_18000c1c0(uVar4,3,&UNK_18014fad8,&UNK_18014efc8,0x484);
        }
      }
      uVar4 = FUN_1800115c0(&stack0x000006d8,&stack0x00000f28,&UNK_18014fb10);
      FUN_18000fb70(&stack0x00000f28,uVar4);
      FUN_180009450(&stack0x000006d8);
      uVar4 = func_0x0001800095f0(&stack0x00000f28);
      uVar5 = FUN_180021530();
      iVar3 = func_0x000180024560(uVar5,uVar4);
      func_0x00018011e414(&stack0x00006374,0,0x98);
      strcpy_s(&stack0x00006374,0x40,&UNK_18014b0d8);
      puVar8 = &UNK_18014c9c8;
      if (iVar3 == 0) {
        puVar8 = &UNK_18014b0e4;
      }
      strcpy_s(&stack0x000063bc,0x40,puVar8);
      uVar2 = func_0x000180009600(&stack0x00000f28);
      puVar6 = (undefined1 *)func_0x00018011dc64(uVar2,0x180197811);
      puVar7 = puVar6;
      if (puVar6 == (undefined1 *)0x0) {
        cVar1 = FUN_18000c2c0(4);
        if (cVar1 != '\0') {
          uVar4 = FUN_18000bfe0();
          FUN_18000c1c0(uVar4,4,&UNK_18014fa30,&UNK_18014efc8,0x49d);
        }
        in_stack_00000158 = 0xe0000002;
        puVar7 = (undefined1 *)func_0x00018011e41a(&stack0x00000158,&UNK_18018ea80);
      }
      func_0x00018011e414(puVar7,0,uVar2);
      uVar4 = func_0x000180009600(&stack0x00000f28);
      uVar5 = func_0x0001800095f0(&stack0x00000f28);
      memcpy_s(puVar6,uVar2,uVar5,uVar4);
      iVar3 = FUN_180062d20();
      if (iVar3 != 0) {
        if (puVar6 != (undefined1 *)0x0) {
          ___V_YAXPEAX_Z();
        }
        cVar1 = FUN_18000c2c0(4);
        if (cVar1 != '\0') {
          uVar4 = FUN_18000bfe0();
          FUN_18000c1c0(uVar4,4,&UNK_18014fa80,&UNK_18014efc8,0x4ad);
        }
        puVar6 = &stack0x000003a4;
        func_0x00018011e41a(puVar6,&UNK_18018ea80);
      }
      if (puVar6 != (undefined1 *)0x0) {
        ___V_YAXPEAX_Z();
      }
      cVar1 = FUN_18000c2c0(2);
      if (cVar1 != '\0') {
        uVar4 = FUN_18000bfe0();
        FUN_18000c1c0(uVar4,2,&UNK_18014fb88,&UNK_18014efc8,0x4b6);
      }
      FUN_180009450(&stack0x00000f28);
    }
  }
  FUN_180009450(&stack0x00000eb8);
  FUN_180009450(&stack0x00000e58);
  FUN_180027d20(&stack0x00000db0);
  cVar1 = FUN_18000c2c0(1);
  if (cVar1 != '\0') {
    uVar4 = FUN_18000bfe0();
    FUN_18000c1c0(uVar4,1,&UNK_180153f18,&UNK_18014efc8,0x1dce);
  }
  FUN_18011da90(in_stack_0001f880 ^ (ulonglong)&stack0x00000000);
  return;
}

