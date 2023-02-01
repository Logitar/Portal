<template>
  <b-container>
    <h1 v-t="'senders.title'" />
    <div class="my-2">
      <icon-button class="mx-1" icon="sync-alt" :loading="loading" text="actions.refresh" variant="primary" @click="refresh()" />
      <icon-button class="mx-1" :href="createUrl" icon="plus" text="actions.create" variant="success" />
    </div>
    <b-row>
      <search-field class="col" v-model="search" />
      <realm-select class="col" v-model="realm" />
      <provider-select class="col" v-model="provider" />
    </b-row>
    <b-row>
      <sort-select class="col" :desc="desc" :options="sortOptions" v-model="sort" @desc="desc = $event" />
      <count-select class="col" v-model="count" />
    </b-row>
    <template v-if="senders.length">
      <table id="table" class="table table-striped">
        <thead>
          <tr>
            <th scope="col" v-t="'senders.emailAddress.label'" />
            <th scope="col" v-t="'senders.displayName.label'" />
            <th scope="col" v-t="'senders.provider.label'" />
            <th scope="col" v-t="'updated'" />
            <th scope="col" />
          </tr>
        </thead>
        <tbody>
          <tr v-for="sender in senders" :key="sender.id">
            <td>
              <b-link :href="`/senders/${sender.id}`">{{ sender.emailAddress }}</b-link>
            </td>
            <td v-text="sender.displayName || '—'" />
            <td>{{ $t(`senders.provider.options.${sender.provider}`) }}</td>
            <td><status-cell :actor="sender.updatedBy" :date="sender.updatedOn" /></td>
            <td>
              <icon-button v-if="sender.isDefault" class="mx-1" disabled icon="star" text="senders.default" variant="info" />
              <icon-button v-else class="mx-1" icon="star" :loading="loading" text="senders.default" variant="warning" @click="onSetDefault(sender)" />
              <icon-button
                class="mx-1"
                :disabled="sender.isDefault && senders.length > 1"
                icon="trash-alt"
                text="actions.delete"
                variant="danger"
                v-b-modal="`delete_${sender.id}`"
              />
              <delete-modal
                confirm="senders.delete.confirm"
                :displayName="sender.displayName ? `${sender.displayName} <${sender.emailAddress}>` : sender.emailAddress"
                :id="`delete_${sender.id}`"
                :loading="loading"
                title="senders.delete.title"
                @ok="onDelete(sender, $event)"
              />
            </td>
          </tr>
        </tbody>
      </table>
      <b-pagination v-model="page" :total-rows="total" :per-page="count" aria-controls="table" />
    </template>
    <p v-else v-t="'senders.empty'" />
  </b-container>
</template>

<script>
import ProviderSelect from './ProviderSelect.vue'
import RealmSelect from '@/components/Realms/RealmSelect.vue'
import { deleteSender, getSenders, setDefault } from '@/api/senders'
import { getQueryString } from '@/helpers/queryUtils'

export default {
  name: 'SenderList',
  components: {
    ProviderSelect,
    RealmSelect
  },
  data() {
    return {
      count: 10,
      desc: false,
      loading: false,
      page: 1,
      provider: null,
      realm: null,
      search: null,
      senders: [],
      sort: 'DisplayName',
      total: 0
    }
  },
  computed: {
    createUrl() {
      return '/create-sender' + getQueryString({ provider: this.provider, realm: this.realm })
    },
    params() {
      return {
        provider: this.provider,
        realm: this.realm,
        search: this.search,
        sort: this.sort,
        desc: this.desc,
        index: (this.page - 1) * this.count,
        count: this.count
      }
    },
    sortOptions() {
      return this.orderBy(
        Object.entries(this.$i18n.t('senders.sort.options')).map(([value, text]) => ({ text, value })),
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
          await deleteSender(id)
          refresh = true
          this.toast('success', 'senders.delete.success')
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
    async onSetDefault({ id }) {
      let refresh = false
      if (!this.loading) {
        this.loading = true
        try {
          await setDefault(id)
          refresh = true
          this.toast('success', 'senders.updated')
        } catch (e) {
          this.handleError(e)
        } finally {
          this.loading = false
        }
      }
      if (refresh) {
        this.refresh()
      }
    },
    async refresh(params = null) {
      if (!this.loading) {
        this.loading = true
        try {
          const { data } = await getSenders(params ?? this.params)
          this.senders = data.items
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
          (newValue.provider !== oldValue.provider ||
            newValue.realm !== oldValue.realm ||
            newValue.search !== oldValue.search ||
            newValue.count !== oldValue.count)
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
