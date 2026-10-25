
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

void VM_M_AsyncCloseSolution(void)

{
  code *pcVar1;
  char cVar2;
  undefined8 uVar3;
  longlong *plVar4;
  undefined1 auStack_88 [32];
  undefined4 uStack_68;
  undefined8 *puStack_60;
  longlong alStack_58 [3];
  longlong *plStack_40;
  undefined8 auStack_38 [3];
  ulonglong uStack_20;
  ulonglong uStack_18;
  
                    /* 0x13d550  22  VM_M_AsyncCloseSolution */
  uStack_18 = _DAT_1802a5230 ^ (ulonglong)auStack_88;
  cVar2 = FUN_18001dbc0(2);
  if (cVar2 != '\0') {
    uVar3 = FUN_18001db50();
    uStack_68 = 0x492;
    FUN_180015b40(uVar3,2,&UNK_1802374a0,&UNK_180237488);
  }
  if ((_DAT_1802b12c0 & 1) == 0) {
    _DAT_1802b12c0 = _DAT_1802b12c0 | 1;
    FUN_18003fde0();
    FUN_1801e8930(&UNK_180202970);
  }
  plStack_40 = (longlong *)0x0;
  plVar4 = (longlong *)__2_YAPEAX_K_Z(0x20);
  if (plVar4 == (longlong *)0x0) {
    __Xbad_alloc_std__YAXXZ();
    pcVar1 = (code *)swi(3);
    (*pcVar1)();
    return;
  }
  *plVar4 = (longlong)&UNK_1802397a8;
  plVar4[1] = (longlong)FUN_180047570;
  plVar4[2] = (longlong)&DAT_1802b12d0;
  plStack_40 = plVar4;
  if ((_DAT_1802b2980 & 1) == 0) {
    _DAT_1802b2980 = _DAT_1802b2980 | 1;
    FUN_18015a9d0();
    FUN_1801e8930(FUN_180202990);
  }
  uStack_20 = 0;
  FUN_1800f3be0(auStack_38,alStack_58);
  FUN_1800fb9d0(0x1802b2998,auStack_38);
  if (plStack_40 != (longlong *)0x0) {
    (**(code **)(*plStack_40 + 0x20))
              (plStack_40,CONCAT71((int7)((ulonglong)alStack_58 >> 8),plStack_40 != alStack_58));
  }
  cVar2 = FUN_18001dbc0(2);
  if (cVar2 != '\0') {
    puStack_60 = (undefined8 *)FUN_18008f750(auStack_38,0);
    if (0xf < (ulonglong)puStack_60[3]) {
      puStack_60 = (undefined8 *)*puStack_60;
    }
    uVar3 = FUN_18001db50();
    uStack_68 = 0x498;
    FUN_180015b40(uVar3,2,&UNK_180234258,&UNK_180237488);
    if (0xf < uStack_20) {
      __3_YAXPEAX_Z(auStack_38[0]);
    }
  }
  FUN_1801e8810(uStack_18 ^ (ulonglong)auStack_88);
  return;
}

