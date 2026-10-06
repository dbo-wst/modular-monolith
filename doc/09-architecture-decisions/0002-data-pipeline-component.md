# 2. Data pipeline component

Date: 2026-03-23

## Status

Accepted

## Context

Drop copies of trades and orders could be delivered in fallacious format.

## Decision

We implement a component that takes the raw drop copies data and transform it into proper format for the further loading into Trios.Core. The rules for transforming should be extracted from "legacy" scripts

## Consequences

Only good data will be imported in Trios automatically. Other data should be ignored or be manually edited
