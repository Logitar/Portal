<template>
  <b-container>
    <h1 v-t="'apiKeys.listTitle'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" href="/create-api-key" icon="plus" text="actions.create" variant="success" />
    </div>
    <b-row>
      <search-field class="col" v-model="search" />
      <form-select class="col" id="status" label="apiKeys.status.label" :options="statusOptions" placeholder="apiKeys.status.placeholder" v-model="isExpired" />
      <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
      <count-select class="col" v-model="count" />
    </b-row>
    <template v-if="apiKeys.length">
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'apiKeys.title.label'" />
            <th scope="col" v-t="'apiKeys.expiresOn'" />
            <th scope="col" v-t="'updated'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="apiKey in apiKeys" :key="apiKey.id">
            <td>
              <b-link :href="`/api-keys/${apiKey.id}`">{{ apiKey.title }}</b-link>
            </td>
            <td>
              {{ apiKey.expiresOn ? $d(new Date(apiKey.expiresOn), 'medium') : $t('apiKeys.neverExpires') }}
              <b-badge v-if="apiKey.expiresOn < new Date().toISOString()" variant="danger">{{ $t('apiKeys.expired') }}</b-badge>
            </td>
            <td><status-cell :actor="apiKey.updatedBy" :date="apiKey.updatedOn || apiKey.createdOn" /></td>
            <td>
              <icon-button icon="trash-alt" text="actions.delete" variant="danger" v-b-modal="`delete_${apiKey.id}`" />
              <delete-modal
                confirm="apiKeys.delete.confirm"
                :displayName="apiKey.title"
                :id="`delete_${apiKey.id}`"
                :loading="loading"
                title="apiKeys.delete.title"
                @ok="onDelete(apiKey, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </template>
    <p v-else v-t="'apiKeys.empty'" />
  </b-container>
</template>

<script>
import { deleteApiKey, getApiKeys } from '@/api/keys'

export default {
  name: 'ApiKeyList',
  data() {
    return {
      apiKeys: [],
      count: 10,
      desc: false,
      isExpired: null,
      loading: false,
      page: 1,
      search: null,
      sort: 'Title',
      total: 0
    }
  },
  computed: {
    params() {
      let expiredOn = null
      switch (this.isExpired) {
        case 'Expired':
          expiredOn = new Date().toISOString()
          break
        case 'NotExpired':
          expiredOn = new Date(Date.now() + 365 * 24 * 60 * 60 * 1000).toISOString()
          break
      }
      return {
        expiredOn,
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('apiKeys.sort.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    },
    statusOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('apiKeys.status.options')).map(([value, text]) => ({ text, value })),
        'text'
      )
    }
  },
  methods: {
    async onDelete({ id }, callback = null) {
      if (!this.loading) {
        this.loading = true
        let refresh = false
        try {
          await deleteApiKey(id)
          refresh = true
          this.toast('success', 'apiKeys.delete.success')
          if (typeof callback === 'function') {
            callback()
          }
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
        if (refresh) {
          await this.refresh()
        }
      }
      if (callback) {
        callback()
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getApiKeys(params ?? this.params)
          this.apiKeys = data.items
          this.total = data.total
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
    }
  },
  watch: {
    params: {
      deep: true,
      immediate: true,
      async handler(newValue, oldValue) {
        if (
          newValue?.index &&
          oldValue &&
          (newValue.isExpired !== oldValue.isExpired || newValue.search !== oldValue.search || newValue.count !== oldValue.count)
        ) {
          this.page = 1
          await this.refresh()
        } else {
          await this.refresh(newValue)
        }
      }
    }
  }
}
</script>
