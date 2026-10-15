# 8.8 Error Handling

Expected failures are returned as `Fin<T>` (LanguageExt) with errors from `Shared.Kernel/Errors`; hosts map them to HTTP / gRPC error responses.
