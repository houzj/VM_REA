
undefined1 *
FUN_180107380(undefined1 *param_1,undefined4 param_2,undefined8 param_3,undefined8 param_4)

{
  undefined *puVar1;
  undefined8 uVar2;
  undefined8 uVar3;
  
  uVar3 = 0xfffffffffffffffe;
  *(undefined8 *)(param_1 + 0x18) = 0xf;
  *(undefined8 *)(param_1 + 0x10) = 0;
  *param_1 = 0;
  FUN_180006300(param_1,&UNK_18021cf35,0,param_4,0,0xfffffffffffffffe);
  switch(param_2) {
  case 1:
    uVar2 = 3;
    puVar1 = &UNK_180225e64;
    break;
  case 2:
    uVar2 = 5;
    puVar1 = &UNK_180225f90;
    break;
  case 3:
    uVar2 = 6;
    puVar1 = &UNK_1802260f8;
    break;
  case 4:
    uVar2 = 5;
    puVar1 = &UNK_180225850;
    break;
  case 5:
    uVar2 = 8;
    puVar1 = &UNK_180226ad0;
    break;
  case 6:
    uVar2 = 9;
    puVar1 = &UNK_180226b10;
    break;
  case 7:
    uVar2 = 6;
    puVar1 = &UNK_1802260c0;
    break;
  default:
    uVar2 = 7;
    puVar1 = &UNK_18022cd58;
  }
  FUN_180006300(param_1,puVar1,uVar2,param_4,1,uVar3);
  return param_1;
}

