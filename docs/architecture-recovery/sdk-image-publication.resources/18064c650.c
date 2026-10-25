
/* WARNING: Globals starting with '_' overlap smaller symbols at the same address */

undefined8
IMVS_QueryModuleHistoryResult
          (longlong param_1,undefined4 param_2,undefined4 param_3,undefined4 param_4,
          undefined8 param_5,undefined1 param_6,undefined1 param_7,undefined1 param_8,
          undefined1 param_9,undefined4 param_10)

{
  undefined8 *puVar1;
  undefined8 *puVar2;
  undefined8 *puVar3;
  longlong *plVar4;
  undefined8 uVar5;
  
                    /* 0x64c650  628  IMVS_QueryModuleHistoryResult */
  plVar4 = (longlong *)FUN_180645170();
  if (plVar4 != (longlong *)0x0) {
    puVar1 = (undefined8 *)*plVar4;
    puVar2 = (undefined8 *)*puVar1;
    for (puVar3 = puVar2; puVar3 != puVar1; puVar3 = (undefined8 *)*puVar3) {
      if (puVar3[2] == param_1) {
        if (puVar3[3] == 0) {
          return 0xe0000006;
        }
        while( true ) {
          if (puVar2 == puVar1) {
            return 0xe0000006;
          }
          if (puVar2[3] == puVar3[3]) break;
          puVar2 = (undefined8 *)*puVar2;
        }
        if (puVar2[2] == 0) {
          return 0xe0000006;
        }
        if (puVar2[2] != param_1) {
          return 0xe0000006;
        }
        if ((_DAT_180ca02d8 & 1) == 0) {
          _DAT_180ca02d8 = _DAT_180ca02d8 | 1;
          FUN_180660680();
          FUN_1809e7f58(&UNK_180a2a8b0);
        }
        uVar5 = FUN_18069df50(&DAT_180ca02e0,param_2,param_3,param_4,param_5,param_6,param_7,param_8
                              ,param_9,param_10);
        return uVar5;
      }
    }
  }
  return 0xe0000006;
}

