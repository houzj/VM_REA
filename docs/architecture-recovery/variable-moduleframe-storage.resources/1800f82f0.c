
void FUN_1800f82f0(longlong param_1,undefined4 param_2,undefined8 *param_3)

{
  char cVar1;
  undefined8 uVar2;
  
  switch(param_2) {
  case 1:
    FUN_1800f9aa0(param_1,param_3);
    return;
  case 2:
    FUN_1800f9aa0(param_1 + 0x10,param_3);
    return;
  case 3:
    FUN_1800f9aa0(param_1 + 0x30,param_3);
    return;
  case 4:
    FUN_1800f9aa0(param_1 + 0x40,param_3);
    return;
  case 5:
    FUN_1800f9aa0(param_1 + 0x50,param_3);
    return;
  case 6:
    FUN_1800f9aa0(param_1 + 0x60,param_3);
    return;
  case 7:
    FUN_1800f9aa0(param_1 + 0x20,param_3);
    return;
  }
  cVar1 = FUN_18001dbc0(4);
  if (cVar1 != '\0') {
    if (0xf < (ulonglong)param_3[3]) {
      param_3 = (undefined8 *)*param_3;
    }
    uVar2 = FUN_18001db50();
    FUN_180015b40(uVar2,4,&DAT_18022ed40,&DAT_18022ecf0,0x581,param_3);
  }
  return;
}

