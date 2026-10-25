
void FUN_1800f4480(undefined8 *param_1)

{
  char cVar1;
  int iVar2;
  undefined8 uVar3;
  undefined8 *puVar4;
  
  cVar1 = FUN_18001dbc0(1);
  if (cVar1 != '\0') {
    puVar4 = param_1;
    if (0xf < (ulonglong)param_1[3]) {
      puVar4 = (undefined8 *)*param_1;
    }
    uVar3 = FUN_18001db50();
    FUN_180015b40(uVar3,1,&UNK_18022e750,&UNK_18022e700,0x23,puVar4,param_1[4],param_1[6],param_1[5]
                 );
  }
  if ((param_1[6] != 0) && (iVar2 = UnmapViewOfFile(), iVar2 == 0)) {
    GetLastError();
  }
  if ((param_1[4] != 0) && (iVar2 = CloseHandle(), iVar2 == 0)) {
    GetLastError();
  }
  if (0xf < (ulonglong)param_1[3]) {
    __3_YAXPEAX_Z(*param_1);
  }
  param_1[3] = 0xf;
  param_1[2] = 0;
  *(undefined1 *)param_1 = 0;
  return;
}

