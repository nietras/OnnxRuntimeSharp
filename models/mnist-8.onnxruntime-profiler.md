# `models\mnist-8.onnx` (26454 bytes)

## Single-request performance
```
Configuration           ;BatchSize;Create [ms];First [ms];Iterations;Mean/b [ms];Mean/s [ms]
Default                 ;        1;      2.362;     0.130;     22400;      0.045;      0.045
1 intra-op thread       ;        1;      2.343;     0.151;     12413;      0.081;      0.081
Wrote ONNX Runtime trace: 'C:\git\oss\OnnxRuntimeSharp\models\mnist-8-onnxruntime-profile-1 intra-op thread_2026-07-30_15-52-32_472.json'.
```

## Concurrent app-thread scaling (single shared session)
```
Configuration           ;Threads;Iterations;Throughput [calls/s];Min Mean/call [ms];Avg Mean/call [ms];Max Mean/call [ms]
Default                 ;      1;     23057;             23032.5;             0.043;             0.043;             0.043
Default                 ;      2;     41521;             41495.7;             0.047;             0.048;             0.049
Default                 ;      4;     79791;             79711.4;             0.049;             0.050;             0.050
Default                 ;      8;    157136;            156980.2;             0.049;             0.051;             0.061
Default                 ;     16;    246250;            245895.0;             0.064;             0.065;             0.067
1 intra-op thread       ;      1;     39329;             39290.2;             0.025;             0.025;             0.025
1 intra-op thread       ;      2;     75647;             75622.1;             0.026;             0.026;             0.027
1 intra-op thread       ;      4;    121365;            121308.5;             0.032;             0.033;             0.034
1 intra-op thread       ;      8;    257730;            257450.1;             0.029;             0.031;             0.032
1 intra-op thread       ;     16;    350335;            350152.4;             0.045;             0.046;             0.047
```

## CPU node profile: `1 intra-op thread`
```
Node                     ;Calls;Total [ms];Mean [ms/call]
Pooling160_Output_0_nchwc;12427;    52.633;        0.004
Pooling66                ;12427;    66.106;        0.005
ReLU114_Output_0_nchwc   ;12427;   198.427;        0.016
ReLU32_Output_0_nchwc    ;12427;   115.915;        0.009
ReorderOutput            ;12427;    54.494;        0.004
ReorderOutput_token_5    ;12427;    51.494;        0.004
Times212/MatMulAddFusion ;12427;    54.621;        0.004
Times212_reshape0        ;12427;    53.686;        0.004
```
