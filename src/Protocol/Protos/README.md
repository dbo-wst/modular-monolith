# Protos

Protocol Buffers sources for `Template.Core.Protocol`. This folder is the `ProtoRoot`, so imports are written relative to it (e.g. `import "prelaunch/ordering/order/entities.proto";`).

## Contents

| Item | Kind | Description |
|---|---|---|
| `prelaunch/ordering/order/` | proto package | `template.core.protocol.prelaunch.ordering.order` (C# `Template.Core.Protocol.Prelaunch.Ordering.Order`): `OrderService`. |
| `prelaunch/ordering/order_item/` | proto package | `template.core.protocol.prelaunch.ordering.order_item` (C# `Template.Core.Protocol.Prelaunch.Ordering.OrderItem`): `OrderItemService`. |

Files per package: `entities.proto` (shared types), `service.proto` (service definition) and one file per operation (`create-*`, `get-*`, `get-*s`, `update-*`, `delete-*`).

## How it works

**`order` package**

- `entities.proto`: `enum Status { STATUS_UNSPECIFIED, NEW, IN_PROGRESS, COMPLETED, CANCELLED, REJECTED }`; `message Order { bytes order_id; optional string number; Timestamp effective_date; optional Status status; }`.
- `service OrderService`:

| RPC | Request | Success branch |
|---|---|---|
| `CreateOrder` | `bytes order_id` (unused by current mappers), `optional string number`, `Timestamp effective_date` | `success: CreateOrderSuccess { Order order }` |
| `GetOrder` | `bytes order_id` | `success: GetOrderSuccess { Order order }` |
| `GetOrders` | `int32 page_number, page_size` | `success: GetOrdersSuccess { repeated Order orders; page_number; page_size }` |
| `UpdateOrder` | `bytes order_id`, `optional number`, `effective_date`, `optional Status status` | `success: UpdateOrderSuccess { Order order }` |
| `DeleteOrder` | `bytes order_id` | `deleted: DeleteOrderSuccess { bytes order_id }` |

**`order_item` package**

- `entities.proto`: `message OrderItem { bytes order_item_id; optional bytes order_id; optional string quantity; optional string price; }` (decimals as invariant-culture strings).
- `service OrderItemService`:

| RPC | Request | Success branch |
|---|---|---|
| `CreateOrderItem` | `optional bytes order_id`, `optional string quantity`, `optional string price` | `success: CreateOrderItemSuccess { OrderItem order_item }` |
| `GetOrderItem` | `bytes order_item_id` | `success: GetOrderItemSuccess { OrderItem order_item }` |
| `GetOrderItems` | `int32 page_number, page_size` | `success: GetOrderItemsSuccess { repeated OrderItem items; page_number; page_size }` |
| `UpdateOrderItem` | `bytes order_item_id`, `optional order_id`, `optional quantity`, `optional price` | `success: UpdateOrderItemSuccess { OrderItem order_item }` |
| `DeleteOrderItem` | `bytes order_item_id` | `deleted: DeleteOrderItemSuccess { bytes order_item_id }` |

**Conventions**
- Every `<Op>Response` is `oneof result { <Op>Success success|deleted = 1; <Op>Error error = 2; }`.
- `<Op>Error { string code; string message; }`; `code` is one of `not_found`, `validation`, `conflict`, `cancelled`, `internal` (set by `ErrorCodeClassifier` in `Template.Core.Api`).
- Ids are `bytes` (convert with `BytesExtension`); dates are `google.protobuf.Timestamp` (convert with `TimestampExtension`).
- `optional` marks "may be absent / not updated" and generates `Has*` properties.

## Dependencies
- References: `google/protobuf/timestamp.proto` (well-known types from `Google.Protobuf`).
- Used by: `Template.Core.Protocol.csproj` (code generation).

## Extending
Create `prelaunch/<module>/<resource>/` with `entities.proto`, one `<op>.proto` per operation following the Success/Error/`oneof` convention, and a `service.proto` importing them; keep `package template.core.protocol.prelaunch.<module>.<resource>` and the matching `csharp_namespace`; register all files in the csproj. Never renumber existing fields.
