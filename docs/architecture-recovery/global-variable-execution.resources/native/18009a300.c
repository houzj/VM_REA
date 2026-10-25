
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void FUN_18009a300(undefined8 param_1,undefined4 param_2,longlong param_3)

{
  ulonglong uVar1;
  code *pcVar2;
  char cVar3;
  undefined1 *puVar4;
  undefined8 uVar5;
  int iVar6;
  longlong lVar7;
  undefined1 auStack_1a8 [32];
  undefined4 uStack_188;
  undefined1 *puStack_180;
  undefined4 uStack_178;
  undefined8 uStack_170;
  undefined4 uStack_168;
  undefined4 uStack_164;
  undefined1 *puStack_160;
  undefined4 uStack_158;
  undefined1 uStack_c8;
  undefined7 uStack_c7;
  ulonglong uStack_b8;
  ulonglong uStack_b0;
  undefined1 auStack_a8 [128];
  ulonglong uStack_28;
  
  uStack_170 = 0xfffffffffffffffe;
  uStack_28 = _DAT_1802a5230 ^ (ulonglong)auStack_1a8;
  uStack_178 = 0;
  if (0 < *(int *)(param_3 + 0x40)) {
    func_0x0001801e9194(&uStack_168,0,0x98);
    uStack_164 = 0x407;
    uStack_168 = param_2;
    puStack_160 = auStack_a8;
    uStack_158 = FUN_180070bc0(auStack_a8,&UNK_180226d80,param_3);
    cVar3 = FUN_18001dbc0(0);
    if (cVar3 != '\0') {
      uStack_b0 = 0xf;
      uStack_b8 = 0;
      uStack_c8 = 0;
      FUN_180006300(&uStack_c8,&UNK_18021cf35,0);
      iVar6 = 0;
      if (0 < *(int *)(param_3 + 0x40)) {
        lVar7 = 0;
        do {
          FUN_18001f7a0(&uStack_c8,
                        *(undefined8 *)
                         (*(longlong *)(lVar7 + 8 + *(longlong *)(param_3 + 0x48)) + 0x18),0,
                        0xffffffffffffffff);
          FUN_18001f5e0(&uStack_c8,&UNK_1802254fc);
          iVar6 = iVar6 + 1;
          lVar7 = lVar7 + 8;
        } while (iVar6 < *(int *)(param_3 + 0x40));
      }
      uVar1 = uStack_b8 - 1;
      if (uStack_b8 < uVar1) {
        __Xout_of_range_std__YAXPEBD_Z(&UNK_18021a360);
        pcVar2 = (code *)swi(3);
        (*pcVar2)();
        return;
      }
      puVar4 = &uStack_c8;
      if (0xf < uStack_b0) {
        puVar4 = (undefined1 *)CONCAT71(uStack_c7,uStack_c8);
      }
      uStack_b8 = uVar1;
      puVar4[uVar1] = 0;
      cVar3 = FUN_18001dbc0(0);
      if (cVar3 != '\0') {
        puVar4 = &uStack_c8;
        if (0xf < uStack_b0) {
          puVar4 = (undefined1 *)CONCAT71(uStack_c7,uStack_c8);
        }
        uVar5 = FUN_18001db50();
        uStack_188 = 0x5f5;
        puStack_180 = puVar4;
        FUN_180015b40(uVar5,0,&UNK_180226ca0,&UNK_180226cc8);
      }
      if (0xf < uStack_b0) {
        __3_YAXPEAX_Z(CONCAT71(uStack_c7,uStack_c8));
      }
      uStack_b0 = 0xf;
      uStack_b8 = 0;
      uStack_c8 = 0;
    }
    if (_DAT_1802a5258 != (code *)0x0) {
      (*_DAT_1802a5258)(7,&uStack_168,_DAT_1802b1180);
    }
    cVar3 = FUN_18001dbc0(1);
    if (cVar3 != '\0') {
      uVar5 = FUN_18001db50();
      uStack_188 = 0x5f9;
      puStack_180 = auStack_a8;
      FUN_180015b40(uVar5,1,&UNK_180226d08,&UNK_180226cc8);
    }
  }
  FUN_1801e8810(uStack_28 ^ (ulonglong)auStack_1a8);
  return;
}

