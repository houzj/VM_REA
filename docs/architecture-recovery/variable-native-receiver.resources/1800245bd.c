
/* WARNING: Function: __security_check_cookie replaced with injection: security_check_cookie */

int FUN_1800245bd(void)

{
  uint uVar1;
  int iVar2;
  undefined8 *puVar3;
  longlong lVar4;
  ulonglong uVar5;
  __uint64 _Var6;
  rsize_t _MaxCount;
  longlong lVar7;
  rsize_t _DstSize;
  undefined1 *puVar8;
  longlong in_stack_00000058;
  int in_stack_00000060;
  int in_stack_00000108;
  uint *in_stack_000001d8;
  uint *puVar9;
  longlong in_stack_000019b0;
  
  puVar3 = (undefined8 *)
           std::shared_ptr<__ExceptionPtr>::shared_ptr<__ExceptionPtr>
                     ((shared_ptr<__ExceptionPtr> *)&stack0x00000e30,
                      (shared_ptr<class___ExceptionPtr> *)&stack0x00000048);
  in_stack_00000060 = FUN_180028110(in_stack_000019b0,puVar3);
  if (in_stack_00000060 == 0) {
    lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
    puVar8 = (undefined1 *)(lVar4 + 0x139);
    for (lVar7 = 0x100; lVar7 != 0; lVar7 = lVar7 + -1) {
      *puVar8 = 0;
      puVar8 = puVar8 + 1;
    }
    lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
    puVar8 = (undefined1 *)(lVar4 + 0x129);
    for (lVar7 = 0x10; lVar7 != 0; lVar7 = lVar7 + -1) {
      *puVar8 = 0;
      puVar8 = puVar8 + 1;
    }
    lVar4 = in_stack_00000058 + 0x204;
    lVar7 = FUN_180009510((undefined8 *)&stack0x00000048);
    _snprintf_s((char *)(lVar7 + 0x129),0x10,0xf,"%s",lVar4);
    lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
    puVar8 = (undefined1 *)(lVar4 + 0x139);
    for (lVar7 = 0x100; lVar7 != 0; lVar7 = lVar7 + -1) {
      *puVar8 = 0;
      puVar8 = puVar8 + 1;
    }
    puVar3 = (undefined8 *)
             std::shared_ptr<__ExceptionPtr>::shared_ptr<__ExceptionPtr>
                       ((shared_ptr<__ExceptionPtr> *)&stack0x00000e90,
                        (shared_ptr<class___ExceptionPtr> *)&stack0x00000048);
    FUN_180025c40(in_stack_000019b0,puVar3);
    if (*(int *)(in_stack_00000058 + 0x304) == 0) {
      puVar3 = (undefined8 *)
               std::shared_ptr<__ExceptionPtr>::shared_ptr<__ExceptionPtr>
                         ((shared_ptr<__ExceptionPtr> *)&stack0x00000e50,
                          (shared_ptr<class___ExceptionPtr> *)&stack0x00000048);
      FUN_180027080(in_stack_000019b0,puVar3);
      lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
      *(undefined1 *)(lVar4 + 0x448) = 0;
    }
    else {
      uVar1 = *(uint *)(in_stack_00000058 + 0x304);
      uVar5 = SUB168(ZEXT816(0x130) * ZEXT416(uVar1),0);
      if (SUB168(ZEXT816(0x130) * ZEXT416(uVar1),8) != 0) {
        uVar5 = 0xffffffffffffffff;
      }
      _Var6 = uVar5 + 8;
      if (0xfffffffffffffff7 < uVar5) {
        _Var6 = 0xffffffffffffffff;
      }
      in_stack_000001d8 = (uint *)thunk_FUN_1800502a4(_Var6);
      if (in_stack_000001d8 == (uint *)0x0) {
        puVar9 = (uint *)0x0;
      }
      else {
        *in_stack_000001d8 = uVar1;
        FUN_180050228((longlong)(in_stack_000001d8 + 2),0x130,uVar1,FUN_18001b890);
        puVar9 = in_stack_000001d8 + 2;
      }
      lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
      *(uint **)(lVar4 + 0x470) = puVar9;
      lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
      if (*(longlong *)(lVar4 + 0x470) == 0) {
        in_stack_00000060 = -0x1ffffffe;
      }
      else {
        iVar2 = *(int *)(in_stack_00000058 + 0x304) * 0x130;
        in_stack_00000108 = iVar2;
        lVar7 = FUN_180009510((undefined8 *)&stack0x00000048);
        puVar8 = *(undefined1 **)(lVar7 + 0x470);
        for (lVar4 = (longlong)iVar2; lVar4 != 0; lVar4 = lVar4 + -1) {
          *puVar8 = 0;
          puVar8 = puVar8 + 1;
        }
        _MaxCount = (rsize_t)in_stack_00000108;
        _DstSize = (rsize_t)in_stack_00000108;
        lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
        memcpy_s(*(void **)(lVar4 + 0x470),_DstSize,*(void **)(in_stack_00000058 + 0x308),_MaxCount)
        ;
        lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
        *(undefined1 *)(lVar4 + 0x448) = 1;
        lVar4 = FUN_180009510((undefined8 *)&stack0x00000048);
        *(undefined4 *)(lVar4 + 0x450) = *(undefined4 *)(in_stack_00000058 + 0x304);
      }
    }
  }
  iVar2 = in_stack_00000060;
  FUN_180016480((longlong)&stack0x00000048);
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x00001080);
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x00001060);
  ~basic_string<>((basic_string<char,std::char_traits<char>,std::allocator<char>_> *)
                  &stack0x00001020);
  FUN_18001b230((longlong *)&stack0x00000070);
  return iVar2;
}

